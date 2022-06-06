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

        var releaseNotes = GitVersion.CreateReleaseNotes(commits, 10);

        releaseNotes.Should().Be(@"⭐ Last 10 features:
Feat 1
Feat 2
Feat 3
🐞 Last 10 bug fixes:
Fix 1
Fix 2
Fix 3");
    }
}