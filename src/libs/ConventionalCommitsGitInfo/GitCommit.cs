using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

public class GitCommit : Task, ICancelableTask
{
    private GitExec? ActiveTask { get; set; }
    private bool IsCancelled { get; set; }

    [Required]
    public string ShortShaFormat { get; set; } = string.Empty;

    [Required]
    public string LongShaFormat { get; set; } = string.Empty;

    [Output]
    public string Commit { get; set; } = string.Empty;

    [Output]
    public string Sha { get; set; } = string.Empty;

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

        Commit = RunShowSignatureShort(ShortShaFormat);
        Sha = RunShowSignatureShort(LongShaFormat);

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
}