﻿using ConventionalCommitsGitInfo;
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
        warnings.Should().BeEmpty();
        messages.Should().NotBeEmpty();

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
}