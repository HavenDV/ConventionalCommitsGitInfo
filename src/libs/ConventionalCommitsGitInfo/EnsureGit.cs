using System.Text.RegularExpressions;
using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

public class EnsureGit : Task, ICancelableTask
{
    [Required]
    public string GitMinVersion { get; set; } = string.Empty;

    private GitVersion? GitVersionTask { get; set; }
    private bool IsCancelled { get; set; }

    public void Cancel()
    {
        IsCancelled = true;
        GitVersionTask?.Cancel();
    }

    public override bool Execute()
    {
        if (IsCancelled)
        {
            return false;
        }

        GitVersionTask = new GitVersion
        {
            BuildEngine = BuildEngine,
        };
        if (!GitVersionTask.Execute() ||
            GitVersionTask.ExitCode != 0)
        {
            Log.LogError($"Failed to run `git --version`. Git may not be properly installed: {GitVersionTask.FullConsoleOutput}");
            return false;
        }

        var currentVersion = Version.Parse(Regex.Match(GitVersionTask.FullConsoleOutput, @"\d+\.\d+\.\d+").Value);
        var minVersion = Version.Parse(GitMinVersion);
        if (currentVersion < minVersion)
        {
            Log.LogError($"Required minimum git version is {minVersion} but found {currentVersion}.");
            return false;
        }

        return true;
    }
}