using System.Text.RegularExpressions;
using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

public class GitRepositoryUrl : Task, ICancelableTask
{
    private GitExec? ActiveTask { get; set; }
    private bool IsCancelled { get; set; }

    [Required]
    public string GitRemote { get; set; } = string.Empty;

    [Output]
    public string RepositoryUrl { get; set; } = string.Empty;

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

        RepositoryUrl = RunConfigGetRemote();

        if (string.IsNullOrWhiteSpace(RepositoryUrl))
        {
            Log.LogWarning(
                "", "GI002", "", "", 0, 0, 0, 0,
                $"Could not retrieve repository url for remote '{GitRemote}");
        }

        return true;
    }

    public string RunConfigGetRemote()
    {
        ActiveTask = new GitExec($"config --get remote.{GitRemote}.url")
        {
            BuildEngine = BuildEngine,
            IgnoreExitCode = true,
            StandardErrorImportance = "low",
        };
        if (!ActiveTask.Execute() ||
            ActiveTask.ExitCode != 0)
        {
            return string.Empty;
        }

        return Regex.Replace(ActiveTask.FullConsoleOutput, "://[^/]*@", "://");
    }
}