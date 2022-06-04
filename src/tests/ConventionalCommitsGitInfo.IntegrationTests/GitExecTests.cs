using ConventionalCommitsGitInfo;
using Microsoft.Build.Framework;
using Moq;

namespace H.Generators.IntegrationTests;

[TestClass]
public class GitExecTests
{
    public static string BaseTest(string arguments)
    {
        var buildEngine = new Mock<IBuildEngine>();
        var errors = new List<BuildErrorEventArgs>();
        var warnings = new List<BuildWarningEventArgs>();
        var messages = new List<BuildMessageEventArgs>();
        buildEngine
            .Setup(static x => x.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
            .Callback<BuildMessageEventArgs>(messages.Add);
        buildEngine
            .Setup(static x => x.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()))
            .Callback<BuildWarningEventArgs>(warnings.Add);
        buildEngine
            .Setup(static x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
            .Callback<BuildErrorEventArgs>(errors.Add);

        var task = new GitExec(arguments)
        {
            BuildEngine = buildEngine.Object,
        };
        var result = task.Execute();

        errors.Should().BeEmpty();
        warnings.Should().BeEmpty();
        //messages.Should().NotBeEmpty();

        result.Should().BeTrue();

        task.ExitCode.Should().Be(0);
        return task.FullConsoleOutput;
    }

    [TestMethod]
    public void GitVersionExecutesCorrectly()
    {
        var output = BaseTest("--version");

        output.Should().StartWith("git version");

        Console.WriteLine($"Output: {output}");
    }

    [TestMethod]
    public void GitShowTopLevelExecutesCorrectly()
    {
        var output = BaseTest("rev-parse --show-toplevel");

        output.Should().NotBeEmpty();

        Console.WriteLine($"Output: {output}");
    }

    [TestMethod]
    public void GitIsInsideWorkTreeExecutesCorrectly()
    {
        var output = BaseTest("rev-parse --is-inside-work-tree");

        output.Should().Be("true");

        Console.WriteLine($"Output: {output}");
    }

    [TestMethod]
    public void GitCommonDirExecutesCorrectly()
    {
        var output = BaseTest("rev-parse --git-common-dir");

        output.Should().EndWith(".git");

        Console.WriteLine($"Output: {output}");
    }

    [TestMethod]
    public void GitConfigRemoteUrlExecutesCorrectly()
    {
        var output = BaseTest("config --get remote.origin.url");

        output.Should().StartWith("https://");

        Console.WriteLine($"Output: {output}");
    }

    [TestMethod]
    public void GitShowCommitDateExecutesCorrectly()
    {
        var output = BaseTest("show --format=%%cI -s");

        output.Should().NotBeEmpty();
        
        Console.WriteLine($"Output: {output}");
    }

    [TestMethod]
    public void GitCommitShortExecutesCorrectly()
    {
        var output = BaseTest("-c log.showSignature=false log --format=format:%%h -n 1");

        output.Should().NotBeEmpty();

        Console.WriteLine($"Output: {output}");
    }

    [TestMethod]
    public void GitCommitLongExecutesCorrectly()
    {
        var output = BaseTest("-c log.showSignature=false log --format=format:%%H -n 1");

        output.Should().NotBeEmpty();

        Console.WriteLine($"Output: {output}");
    }

    //[TestMethod]
    //public void GitRevListExecutesCorrectly()
    //{
    //    var baseCommit = BaseTest("-c log.showSignature=false log --format=format:%%h -n 1");

    //    baseCommit.Should().NotBeEmpty();

    //    var output = BaseTest($"rev-list --max-count 10 {baseCommit} --pretty --date=iso-strict");

    //    output.Should().NotBeEmpty();

    //    Console.WriteLine($"Output: {output}");

    //    var lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    //    var commits = new List<CommitData>();
    //    for (var i = 0; i < lines.Length; i += 4)
    //    {
    //        var text = string.Join(
    //            Environment.NewLine,
    //            lines[i],
    //            lines[i + 1],
    //            lines[i + 2],
    //            lines[i + 3]);

    //        commits.Add(CommitData.Parse(text));
    //    }

    //    Console.WriteLine("Commits:");
    //    foreach (var commit in commits)
    //    {
    //        Console.WriteLine($"Commit: {commit.Commit}");
    //        Console.WriteLine($"Author: {commit.Author}");
    //        Console.WriteLine($"Date: {commit.Date}");
    //        Console.WriteLine($"Message: {commit.Message}");
    //    }

    //    var version = new Version(0, 1, 0);
    //    foreach (var commit in commits)
    //    {
    //        if (commit.IsBreakingChange)
    //        {
    //            version = new Version(version.Major + 1, 0, 0);
    //        }
    //        else if (commit.IsFeature)
    //        {
    //            version = new Version(version.Major, version.Minor + 1, 0);
    //        }
    //        else if (commit.IsFix)
    //        {
    //            version = new Version(version.Major, version.Minor, version.Build + 1);
    //        }
    //    }

    //    Console.WriteLine($"Version: {version}");
    //}
}