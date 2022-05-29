using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

/// <summary>
/// Cascading probing mechanism will try to locate an installed version of git, msysgit, WSL git or cygwin git.
/// </summary>
public class SetGitExe : Task, ICancelableTask
{
    private GitVersion? GitVersionTask { get; set; }
    private bool IsCancelled { get; set; }

    [Output]
    public string Path { get; set; } = string.Empty;

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

        Path = GetGitPath();
        return true;
    }

    public string GetGitPath()
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                GitVersionTask = new GitVersion
                {
                    BuildEngine = BuildEngine,
                    IgnoreExitCode = true,
                    StandardErrorImportance = "low",
                };
                if (GitVersionTask.Execute() &&
                    GitVersionTask.ExitCode == 0)
                {
                    return "git";
                }
                if (File.Exists(@"C:\Program Files\Git\bin\git.exe"))
                {
                    return @"C:\Program Files\Git\bin\git.exe";
                }
                if (File.Exists(@"C:\Program Files (x86)\Git\bin\git.exe"))
                {
                    return @"C:\Program Files (x86)\Git\bin\git.exe";
                }
                if (File.Exists(@"C:\msysgit\bin\git.exe"))
                {
                    return @"C:\msysgit\bin\git.exe";
                }

                throw new InvalidOperationException("git not found.");

            default:
                if (File.Exists("/usr/bin/git"))
                {
                    return "/usr/bin/git";
                }
                if (File.Exists("/usr/local/bin/git"))
                {
                    return "/usr/local/bin/git";
                }

                return "git";
        }
    }
}