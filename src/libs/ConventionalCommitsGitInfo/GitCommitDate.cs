using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

public class GitCommitDate : Task, ICancelableTask
{
    private GitExec? ActiveTask { get; set; }
    private bool IsCancelled { get; set; }

    [Required]
    public string DateFormat { get; set; } = string.Empty;

    [Output]
    public string Date { get; set; } = string.Empty;

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

        Date = RunShow();
        return true;
    }

    public string RunShow()
    {
        ActiveTask = new GitExec($"show --format={DateFormat} -s")
        {
            BuildEngine = BuildEngine,
            IgnoreExitCode = true,
            StandardErrorImportance = "low",
        };
        if (!ActiveTask.Execute() ||
            ActiveTask.ExitCode != 0)
        {
            return "0001-01-01T00:00:00+00:00";
        }

        return ActiveTask.FullConsoleOutput;
    }
}