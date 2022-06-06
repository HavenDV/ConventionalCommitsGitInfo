namespace ConventionalCommitsGitInfo;

public class CommitData
{
    public string Commit { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MessageWithoutType => Message
        .Replace("fix: ", "")
        .Replace("feat: ", "");

    public bool IsFix =>
        Message.StartsWith("fix(") ||
        Message.StartsWith("fix:");

    public bool IsFeature =>
        Message.StartsWith("feat(") ||
        Message.StartsWith("feat:");

    public bool IsBreakingChange =>
        Message.StartsWith("BREAKING CHANGE(") ||
        Message.StartsWith("BREAKING CHANGE:");
        //Message.Contains('!');

    public static CommitData Parse(string text)
    {
        text = text ?? throw new ArgumentNullException(nameof(text));

        return new CommitData
        {
            Commit = text.Substring(0, 40),
            Message = text.Substring(41),
        };
    }
}