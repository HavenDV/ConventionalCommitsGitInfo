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
        //warnings.Should().BeEmpty();
        //messages.Should().NotBeEmpty();

        Console.WriteLine("Warnings:");
        foreach (var warning in warnings)
        {
            Console.WriteLine($"{warning.SenderName}: {warning.Message}");
        }

        Console.WriteLine("Messages:");
        foreach (var message in messages)
        {
            Console.WriteLine($"{message.SenderName}({message.Importance}): {message.Message}");
        }

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

    [TestMethod]
    public void GitLogExecutesCorrectly()
    {
        var output = BaseTest("log --pretty=oneline --no-decorate");

        output.Should().NotBeEmpty();

        Console.WriteLine($"Output: {output}");
    }
}