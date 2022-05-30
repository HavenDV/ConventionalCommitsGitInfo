using ConventionalCommitsGitInfo;
using Microsoft.Build.Framework;
using Moq;

namespace H.Generators.IntegrationTests;

[TestClass]
public class TaskTests
{
    private Mock<IBuildEngine>? BuildEngine { get; set; }
    private List<BuildMessageEventArgs> Messages { get; set; } = new();
    private List<BuildWarningEventArgs> Warnings { get; set; } = new();
    private List<BuildErrorEventArgs> Errors { get; set; } = new();

    [TestInitialize]
    public void Startup()
    {
        BuildEngine = new Mock<IBuildEngine>();
        Errors = new List<BuildErrorEventArgs>();
        Messages = new List<BuildMessageEventArgs>();
        BuildEngine
            .Setup(static x => x.LogMessageEvent(It.IsAny<BuildMessageEventArgs>()))
            .Callback<BuildMessageEventArgs>(Messages.Add);
        BuildEngine
            .Setup(static x => x.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()))
            .Callback<BuildWarningEventArgs>(Warnings.Add);
        BuildEngine
            .Setup(static x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
            .Callback<BuildErrorEventArgs>(Errors.Add);
    }

    [TestMethod]
    public void GitVersionExecutesCorrectly()
    {
        var task = new GitExec("--version")
        {
            BuildEngine = BuildEngine!.Object,
        };
        task.Execute().Should().BeTrue();
        
        Errors.Should().BeEmpty();
        Warnings.Should().BeEmpty();
        Messages.Should().NotBeEmpty();

        task.FullConsoleOutput.Should().StartWith("git version");
        task.ExitCode.Should().Be(0);
    }

    [TestMethod]
    public void EnsureGitExecutesCorrectly()
    {
        var task = new EnsureGit
        {
            BuildEngine = BuildEngine!.Object,
            GitMinVersion = "2.5.0",
        };
        var result = task.Execute();

        Errors.Should().BeEmpty();
        Warnings.Should().BeEmpty();
        Messages.Should().NotBeEmpty();

        result.Should().BeTrue();
    }

    [TestMethod]
    public void SetGitExeExecutesCorrectly()
    {
        var task = new SetGitExe
        {
            BuildEngine = BuildEngine!.Object,
        };
        var result = task.Execute();

        Errors.Should().BeEmpty();
        Warnings.Should().BeEmpty();
        Messages.Should().NotBeEmpty();

        result.Should().BeTrue();

        task.Path.Should().NotBeEmpty();

        Console.WriteLine($"Git path: {task.Path}");
    }
}