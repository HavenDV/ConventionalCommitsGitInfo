using ConventionalCommitsGitInfo.Extensions;
using File = System.IO.File;
using Task = Microsoft.Build.Utilities.Task;

namespace ConventionalCommitsGitInfo;

public class GitRoot : Task, ICancelableTask
{
    private GitExec? ActiveTask { get; set; }
    private bool IsCancelled { get; set; }

    [Required]
    public string IntermediateOutputPath { get; set; } = string.Empty;

    [Required]
    public MessageImportance GitInfoReportImportance { get; set; }

    [Required]
    public string GitInfoBaseDir { get; set; } = string.Empty;

    [Output]
    public string Root { get; set; } = string.Empty;

    [Output]
    public string Dir { get; set; } = string.Empty;

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

        Root = RunTopLevel();

        // Determine the .git dir. In the simple case, this is just $(GitRoot)\.git.
        // But in the case of submodules, a .git* file*rather than a directory
        //    will be present at that path, with a value like:
        // gitdir: ../../.git/modules/external/toq
        // Which points to the actual folder where the git info exists in the containing
        // repository.
        Dir = Path.Combine(Root, ".git");
        var isGitFile = File.Exists(Dir);
        if (isGitFile)
        {
            var isInsideWorkTree = RunIsInsideWorkTree();
            if (isInsideWorkTree)
            {
                Dir = RunCommonDir();
            }
            else
            {
                var content = File.ReadAllText(Dir);
                Dir = content.Substring(7).Trim();
            }
        }

		//<PropertyGroup Condition="'$(_IsGitFile)' == 'true' and '$(_IsGitWorkTree)' != 'true'">
		//	<_GitFileContents>$([System.IO.File]::ReadAllText('$(GitDir)'))</_GitFileContents>
		//	<GitDir>$(_GitFileContents.Substring(7).Trim())</GitDir>
		//	<GitDir>$([MSBuild]::NormalizeDirectory('$(GitRoot)', '$(GitDir)'))</GitDir>
		//</PropertyGroup>

        if (!string.IsNullOrWhiteSpace(Root))
        {
            Log.LogMessage(GitInfoReportImportance, $"Determined Git repository root as '{Root}'");
            Log.LogMessage(GitInfoReportImportance, $"Determined Git dir as '{Dir}'");
        }
        else
        {
            Log.LogWarning(
                "", "GI001", "", "", 0, 0, 0, 0,
                $"Directory {GitInfoBaseDir} is not in a Git repository. Cannot determine Git repository root.");
        }

        var isDirty = RunIsDirty();

        File.WriteAllText(Path.Combine(IntermediateOutputPath, "GitIsDirty.cache"), $"{isDirty}");
        return true;
    }

    public string RunTopLevel()
    {
        ActiveTask = new GitExec("rev-parse --show-toplevel")
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
        
        return ActiveTask.FullConsoleOutput.Trim().NormalizeDirectory();
    }

    public string RunCommonDir()
    {
        ActiveTask = new GitExec("rev-parse --git-common-dir")
        {
            BuildEngine = BuildEngine,
        };
        if (!ActiveTask.Execute() ||
            ActiveTask.ExitCode != 0)
        {
            return string.Empty;
        }

        return ActiveTask.FullConsoleOutput.Trim();
    }

    public bool RunIsInsideWorkTree()
    {
        ActiveTask = new GitExec("rev-parse --is-inside-work-tree")
        {
            BuildEngine = BuildEngine,
            IgnoreExitCode = true,
            StandardErrorImportance = "low",
        };
        if (!ActiveTask.Execute() ||
            ActiveTask.ExitCode != 0)
        {
            return false;
        }

        return ActiveTask.FullConsoleOutput.Trim() == "true";
    }

    public int RunIsDirty()
    {
        ActiveTask = new GitExec("diff --quiet HEAD")
        {
            BuildEngine = BuildEngine,
            IgnoreExitCode = true,
            StandardErrorImportance = "low",
        };
        if (!ActiveTask.Execute() ||
            ActiveTask.ExitCode != 0)
        {
            return 1;
        }

        return 0;
    }
}