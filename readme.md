# ConventionalCommitsGitInfo

## Settings
Default settings are shown here:
```xml
  <Target Name="SetSemanticVersionSettings" BeforeTargets="SetSemanticVersion">
    <PropertyGroup>
        <!-- The version from which automatic versioning will start. -->
        <ConventionalCommitsGitInfo_BaseVersion>0.1.0</ConventionalCommitsGitInfo_BaseVersion>
        <!-- ID of the last commit before automatic versioning started. -->
        <ConventionalCommitsGitInfo_BaseCommit></ConventionalCommitsGitInfo_BaseCommit>
        <!-- Maximum number of countable commits per category. -->
        <ConventionalCommitsGitInfo_ReleaseNotesCount>10</ConventionalCommitsGitInfo_ReleaseNotesCount>
    </PropertyGroup>
  </Target>
```