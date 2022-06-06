using ConventionalCommitsGitInfo;
using Microsoft.Build.Framework;
using Moq;

namespace H.Generators.IntegrationTests;

[TestClass]
public class TaskTests
{
    private static void BaseTest(ITask task)
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

        task.BuildEngine = buildEngine.Object;
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
    }

    [TestMethod]
    public void EnsureGitExecutesCorrectly()
    {
        var task = new EnsureGit
        {
            GitMinVersion = "2.5.0",
        };
        BaseTest(task);
    }

    [TestMethod]
    public void SetGitExeExecutesCorrectly()
    {
        var task = new SetGitExe();
        BaseTest(task);

        task.Path.Should().NotBeEmpty();

        Console.WriteLine($"Git path: {task.Path}");
    }

    [TestMethod]
    public void GitRootExecutesCorrectly()
    {
        var task = new GitRoot();
        BaseTest(task);

        task.Root.Should().NotBeEmpty();
        task.Dir.Should().NotBeEmpty();

        Console.WriteLine($"Root: {task.Root}");
        Console.WriteLine($"Dir: {task.Dir}");
    }

    [TestMethod]
    public void GitRepositoryUrlExecutesCorrectly()
    {
        var task = new GitRepositoryUrl
        {
            GitRemote = "origin",
        };
        BaseTest(task);

        task.RepositoryUrl.Should().NotBeEmpty();

        Console.WriteLine($"RepositoryUrl: {task.RepositoryUrl}");
    }

    [TestMethod]
    public void GitCommitDateExecutesCorrectly()
    {
        var task = new GitCommitDate
        {
            DateFormat = "%%cI",
        };
        BaseTest(task);

        task.Date.Should().NotBeEmpty();

        Console.WriteLine($"Date: {task.Date}");
    }

    [TestMethod]
    public void GitCommitExecutesCorrectly()
    {
        var task = new GitCommit
        {
            ShortShaFormat = "%%h",
            LongShaFormat = "%%H",
        };
        BaseTest(task);

        task.Commit.Should().NotBeEmpty();
        task.Sha.Should().NotBeEmpty();

        Console.WriteLine($"Commit: {task.Commit}");
        Console.WriteLine($"Sha: {task.Sha}");
    }

    [TestMethod]
    public void GitVersionExecutesCorrectly()
    {
        var task = new GitVersion
        {
            BaseVersion = "0.1.0",
            BaseCommit = "9d943258ab8d2c578d97260b8f82eb14118c675e",
        };
        BaseTest(task);

        Console.WriteLine($"Version: {task.Version}");
    }
}