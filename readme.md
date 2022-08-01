# ConventionalCommitsGitInfo

Once installed, this library adds a `SetSemanticVersion` target that sets the following properties:
```xml
<PropertyGroup>
    <Version>$(_Version)</Version>
    <FileVersion>$(Version)</FileVersion>
    <PackageVersion>$(Version)</PackageVersion>
    <PackageReleaseNotes>$(_ReleaseNotes)</PackageReleaseNotes>
</PropertyGroup>
```

Versions will be calculated according to [Conventional Commits specification](https://www.conventionalcommits.org/)
specifications:
- For each `BREAKING CHANGE:` the major version will be increased
- For each `feat:` the minor version will be increased
- For each `fix:` the patch version will be increased
There are two behaviors here, for new libraries with no commit history, and for adapting old libraries.
1. For new libraries versioning starts from version `0.1.0`.
2. For older versions, you need to specify the commit ID from which versioning will start and the base version.
PackageReleaseNotes will contain text like this:
```
⭐ Last 10 features:
Added ability to set up custom OnChanged method.
Added OnChanged/OnChanging implementation detection to remove unused callbacks.
Added possibility of embedding attributes.
🐞 Last 10 bug fixes:
Fixed bug with AttachedDependencyProperty method detection.
Fixed Nullable value type generation.
Fixed issues with BindEvents after detection feature.
```

## Install
[![NuGet](https://img.shields.io/nuget/dt/ConventionalCommitsGitInfo.svg?style=flat-square&label=ConventionalCommitsGitInfo)](https://www.nuget.org/packages/ConventionalCommitsGitInfo/)

```
Install-Package H.IpInfo
```

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
        <!-- Indicates the importance of messages. Use for debugging. -->
        <ConventionalCommitsGitInfo_MessageImportance>low</ConventionalCommitsGitInfo_MessageImportance>
    </PropertyGroup>
  </Target>
```