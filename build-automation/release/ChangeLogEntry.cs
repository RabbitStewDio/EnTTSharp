using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public readonly struct ChangeLogEntry
{
    static readonly Regex CommitMessageRegex = new Regex(@"\[(?<category>[^\]\n]+)\](?<Message>(.*))");

    public readonly string Commit;
    public readonly DateTime CommitTime;
    public readonly bool IsMergeCommit;
    public readonly string Subject;
    public readonly string Author;
    public readonly List<string> Body;
    public readonly string Category;

    public ChangeLogEntry(string commit, DateTime commitTime, bool isMergeCommit, string subject, string author, List<string> body)
    {
        Commit = commit;
        CommitTime = commitTime;
        IsMergeCommit = isMergeCommit;
        Subject = subject;
        Author = author;
        Body = body;
        Category = "???";

        var matches = CommitMessageRegex.Matches(subject);
        foreach (Match m in matches)
        {
            Category = m.Groups["category"].Value.Trim();
            Subject = m.Groups["Message"].Value.Trim();
        }
    }

    public string ToMarkDown()
    {
        var s = $"* {Subject.Trim()}\n";
        if (Body.Count > 0)
        {
            s += $"\n";
            foreach (var b in Body)
            {
                s += "  ";
                s += b;
                s += "\n";
            }
        }
        else
        {
            s += "\n";
        }

        return s;
    }

    public override string ToString()
    {
        if (Body.Count == 0)
        {
            return $@"{nameof(Commit)}: {Commit}
{nameof(CommitTime)}: {CommitTime}
{nameof(IsMergeCommit)}: {IsMergeCommit}
{nameof(Author)}: {Author}
{nameof(Subject)}: {Subject}";
        }

        return $@"{nameof(Commit)}: {Commit}
{nameof(CommitTime)}: {CommitTime}
{nameof(IsMergeCommit)}: {IsMergeCommit}
{nameof(Author)}: {Author}
{nameof(Subject)}: {Subject}
{nameof(Body)}: {string.Join("\n", Body)}";
    }
}
