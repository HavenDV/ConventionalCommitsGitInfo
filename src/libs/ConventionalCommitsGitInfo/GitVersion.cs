namespace ConventionalCommitsGitInfo;

public class GitVersion : GitExec
{
    public GitVersion() : base("--version")
    {
    }
}