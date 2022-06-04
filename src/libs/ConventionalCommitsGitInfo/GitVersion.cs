using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

public class GitVersion : Task, ICancelableTask
{
    private GitExec? ActiveTask { get; set; }
    private bool IsCancelled { get; set; }

    public Version BaseVersion { get; set; } = new(0, 1, 0);

    [Output]
    public Version Version { get; set; } = new(0, 1, 0);

    [Output]
    public string VersionString => $"{Version}";

    [Output]
    public string ReleaseNotes { get; set; } = string.Empty;

    public void Cancel()
    {
        IsCancelled = true;
        ActiveTask?.Cancel();
    }

    public override bool Execute()
    {
        if (IsCancelled)
        {
            return false;
        }

        var baseCommit = RunShowSignatureShort("%%h");
        var commits = RunCommits(baseCommit);

        var version = BaseVersion;
        foreach (var commit in commits)
        {
            if (commit.IsBreakingChange)
            {
                version = new Version(version.Major + 1, 0, 0);
            }
            else if (commit.IsFeature)
            {
                version = new Version(version.Major, version.Minor + 1, 0);
            }
            else if (commit.IsFix)
            {
                version = new Version(version.Major, version.Minor, version.Build + 1);
            }
        }
        Version = version;

        ReleaseNotes = @$"⭐ Last 10 features:
{string.Join(Environment.NewLine, commits
    .Where(static commit => commit.IsFeature)
    .Select(static commit => $"{commit.Date}: {commit.Message}"))}
🐞 Last 10 bug fixes:
{string.Join(Environment.NewLine, commits
    .Where(static commit => commit.IsFix)
    .Select(static commit => $"{commit.Date}: {commit.Message}"))}";

        return true;
    }

    public string RunShowSignatureShort(string format)
    {
        ActiveTask = new GitExec($"-c log.showSignature=false log --format=format:{format} -n 1")
        {
            BuildEngine = BuildEngine,
            IgnoreExitCode = true,
            StandardErrorImportance = "low",
        };
        if (!ActiveTask.Execute() ||
            ActiveTask.ExitCode != 0)
        {
            return "0000000";
        }

        return ActiveTask.FullConsoleOutput;
    }

    public IReadOnlyCollection<CommitData> RunCommits(string baseCommit)
    {
        ActiveTask = new GitExec($"rev-list {baseCommit} --pretty --date=iso-strict")
        {
            BuildEngine = BuildEngine,
            IgnoreExitCode = true,
            StandardErrorImportance = "low",
        };
        if (!ActiveTask.Execute() ||
            ActiveTask.ExitCode != 0)
        {
            return Array.Empty<CommitData>();
        }

        var lines = ActiveTask.FullConsoleOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var commits = new List<CommitData>();
        for (var i = 0; i < lines.Length; i += 4)
        {
            var text = string.Join(
                Environment.NewLine,
                lines[i],
                lines[i + 1],
                lines[i + 2],
                lines[i + 3]);

            commits.Add(CommitData.Parse(text));
        }

        return commits;
    }
}