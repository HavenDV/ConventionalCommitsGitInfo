namespace ConventionalCommitsGitInfo.Extensions;

internal static class StringExtensions
{
    public static string NormalizeDirectory(this string path)
    {
        return Path
            .GetFullPath(new Uri(path).LocalPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .ToUpperInvariant();
    }
}
