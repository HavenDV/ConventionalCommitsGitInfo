using System.Globalization;

namespace ConventionalCommitsGitInfo;

public class CommitData
{
    public string Commit { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool IsFix =>
        Message.StartsWith("fix(") ||
        Message.StartsWith("fix:");

    public bool IsFeature =>
        Message.StartsWith("feat(") ||
        Message.StartsWith("feat:");

    public bool IsBreakingChange =>
        Message.StartsWith("BREAKING CHANGE(") ||
        Message.StartsWith("BREAKING CHANGE:") ||
        Message.Contains('!');

    public static CommitData Parse(string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        return new CommitData
        {
            Commit = lines.ElementAt(0).Substring(7),
            Author = lines.ElementAt(1).Substring(8),
            Date = DateTime.Parse(lines.ElementAt(2).Substring(8), CultureInfo.InvariantCulture),
            Message = lines.ElementAt(3),
        };
    }
}