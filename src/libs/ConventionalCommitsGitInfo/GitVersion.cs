using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

public class GitVersion : Task, ICancelableTask
{
    private GitExec? ActiveTask { get; set; }
    private bool IsCancelled { get; set; }

    public string BaseVersion { get; set; } = string.Empty;

    public string BaseCommit { get; set; } = string.Empty;

    public int ReleaseNotesCount { get; set; }

    [Output]
    public string Version { get; set; } = string.Empty;

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

        var commits = RunCommits("");

        var baseVersion = new Version(BaseVersion);
        var version = CalculateVersion(commits
            .TakeWhile(commit => string.IsNullOrWhiteSpace(BaseCommit) || !commit.Commit.StartsWith(BaseCommit))
            .Reverse()
            .ToArray(), baseVersion);
        Version = $"{version}";
        ReleaseNotes = CreateReleaseNotes(commits, ReleaseNotesCount);

        return true;
    }

    public static Version CalculateVersion(IReadOnlyCollection<CommitData> commits, Version baseVersion)
    {
        commits = commits ?? throw new ArgumentNullException(nameof(commits));
        baseVersion = baseVersion ?? throw new ArgumentNullException(nameof(baseVersion));

        var version = baseVersion;
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

        return version;
    }

    public static string CreateReleaseNotes(IReadOnlyCollection<CommitData> commits, int count)
    {
        commits = commits ?? throw new ArgumentNullException(nameof(commits));

        return @$"⭐ Last {count} features:
{string.Join(Environment.NewLine, commits
    .Where(static commit => commit.IsFeature)
    .Take(count)
    .Select(static commit => $"{commit.MessageWithoutType}"))}
🐞 Last {count} fixes:
{string.Join(Environment.NewLine, commits
    .Where(static commit => commit.IsFix)
    .Take(count)
    .Select(static commit => $"{commit.MessageWithoutType}"))}";
    }

    public IReadOnlyCollection<CommitData> RunCommits(string baseCommit)
    {
        var arguments = "log --pretty=oneline --no-decorate --date=iso-strict";
        if (!string.IsNullOrWhiteSpace(baseCommit))
        {
            arguments += $" {baseCommit}..";
        }
        ActiveTask = new GitExec(arguments)
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

        return ActiveTask.FullConsoleOutput
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(CommitData.Parse)
            .ToArray();
    }
}