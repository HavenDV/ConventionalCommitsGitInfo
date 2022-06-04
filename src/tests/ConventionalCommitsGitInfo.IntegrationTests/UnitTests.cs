using ConventionalCommitsGitInfo;

namespace H.Generators.IntegrationTests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public void CalculatesVersionCorrectly()
    {
        var commits = new[]
        {
            new CommitData
            {
                Message = "feat: Feat 1",
            },
            new CommitData
            {
                Message = "fix: Fix 1",
            },
            new CommitData
            {
                Message = "feat: Feat 2",
            },
            new CommitData
            {
                Message = "fix: Fix 2",
            },
            new CommitData
            {
                Message = "feat: Feat 3",
            },
            new CommitData
            {
                Message = "fix: Fix 3",
            },
        };

        var version = GitVersion.CalculateVersion(commits, new Version(0, 1, 0));

        version.Should().Be(new Version(0, 4, 1));
    }

    [TestMethod]
    public void CreatesReleaseNotesCorrectly()
    {
        var commits = new[]
        {
            new CommitData
            {
                Message = "feat: Feat 1",
            },
            new CommitData
            {
                Message = "fix: Fix 1",
            },
            new CommitData
            {
                Message = "feat: Feat 2",
            },
            new CommitData
            {
                Message = "fix: Fix 2",
            },
            new CommitData
            {
                Message = "feat: Feat 3",
            },
            new CommitData
            {
                Message = "fix: Fix 3",
            },
        };

        var releaseNotes = GitVersion.CreateReleaseNotes(commits);

        releaseNotes.Should().Be(@"⭐ Last 10 features:
1/1/0001: feat: Feat 1
1/1/0001: feat: Feat 2
1/1/0001: feat: Feat 3
🐞 Last 10 bug fixes:
1/1/0001: fix: Fix 1
1/1/0001: fix: Fix 2
1/1/0001: fix: Fix 3");
    }
}