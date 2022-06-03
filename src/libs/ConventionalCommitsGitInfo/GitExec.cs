namespace ConventionalCommitsGitInfo;

public class GitExec : Exec
{
    public string FullConsoleOutput => string.Join(Environment.NewLine, ConsoleOutput.Select(static i => i.ItemSpec));

    public GitExec(string arguments)
    {
        EchoOff = true;
        ConsoleToMSBuild = true;
        StandardErrorImportance = "high";
        StandardOutputImportance = "low";
        Command = $"git {arguments}";
        StdOutEncoding = "utf-8";
    }
}