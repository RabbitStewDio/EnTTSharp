using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.Git;
using Nuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class ChangeLogGenerator
{
    public static List<string> CollectLogRaw(string baseLine)
    {
        var f = Path.GetTempFileName();
        var formatString = @"%H%n%P%n%aI%n%cn <%ce>%n%s%n%n%w(0,5,5)%b%w(0,0,0)%n--%n";
        if (string.IsNullOrEmpty(baseLine))
        {
            GitTasks.Git($"log --date=iso8601-strict --pretty=tformat:{formatString.DoubleQuoteIfNeeded()} --output={f.DoubleQuoteIfNeeded()}");
        }
        else
        {
            var refLimit = $"{baseLine}..HEAD".DoubleQuoteIfNeeded();
            GitTasks.Git($"log {refLimit} --date=iso8601-strict --pretty=tformat:{formatString.DoubleQuoteIfNeeded()} --output={f.DoubleQuoteIfNeeded()}");
        }

        Logger.Info("Writing to " + f);
        var lines = File.ReadLines(f).ToList();
        File.Delete(f);
        return lines;
    }


    public static IEnumerable<ChangeLogEntry> CollectLog(string baselineCommit = null)
    {
        var raw = CollectLogRaw(baselineCommit);
        Logger.Warn("Read " + raw.Count + " lines");
        IEnumerator<string> sr = raw.GetEnumerator();
        while (true)
        {
            if (!TryReadLine(sr, out var commitHash))
            {
                yield break;
            }

            if (!TryReadLine(sr, out var parentHashes))
            {
                yield break;
            }

            var isMergeCommit = parentHashes.Contains(" ");

            if (!TryReadLine(sr, out var dateText))
            {
                yield break;
            }

            if (!DateTime.TryParse(dateText, null, DateTimeStyles.RoundtripKind, out var date))
            {
                yield break;
            }

            if (!TryReadLine(sr, out var authorText))
            {
                yield break;
            }

            if (!TryReadLine(sr, out var subjectText))
            {
                yield break;
            }

            var eol = ParseBody(sr, out var body);
            yield return new ChangeLogEntry(commitHash, date, isMergeCommit, subjectText, authorText, body);
            if (eol)
            {
                yield break;
            }
        }
    }

    static bool ParseBody(IEnumerator<string> r, out List<string> body)
    {
        var versionIncrementComment = new Regex("\\+semver:\\s?(breaking|major|feature|minor|fix|patch)");
        
        var lines = new List<string>();
        bool eol = false;
        bool skip = false;
        while (true)
        {
            if (!r.MoveNext())
            {
                eol = true;
                break;
            }

            var l = r.Current;
            if (l == null)
            {
                eol = true;
                break;
            }

            if ("--".Equals(l))
            {
                break;
            }

            if ("--".Equals(l.Trim()))
            {
                skip = true;
                continue;
            }

            if (versionIncrementComment.IsMatch(l))
            {
                continue;
            }

            if (skip)
            {
                continue;
            }
            
            if (lines.Count == 0 && string.IsNullOrWhiteSpace(l))
            {
                continue;
            }

            lines.Add(l.Trim());
        }

        body = lines;
        return eol;
    }

    public static bool TryReadLine(IEnumerator<string> r, out string line)
    {
        while (r.MoveNext())
        {
            line = r.Current;
            if (!string.IsNullOrEmpty(line))
            {
                return true;
            }
        }

        line = default;
        return false;
    }

    public static (string changeLog, string currentReleaseSection) UpdateChangeLogFile(AbsolutePath changeLogFile, string version, string releaseTargetBranch)
    {
        var sb = new StringBuilder();
        var changeLogSectionEntries = ExtractChangelog(changeLogFile);
        foreach (var s in changeLogSectionEntries.content.Take(changeLogSectionEntries.insertIndex))
        {
            sb.AppendLine(s);
        }

        var section = GenerateNewChangeLogSection(version, releaseTargetBranch); 
        sb.Append(section);

        foreach (var s in changeLogSectionEntries.content.Skip(changeLogSectionEntries.insertIndex))
        {
            sb.AppendLine(s);
        }

        return (sb.ToString(), section);
    }


    /// <summary>
    /// Extracts the notes of the specified changelog section.
    /// </summary>
    /// <param name="changelogFile">The path to the changelog file.</param>
    /// <param name="tag">The tag which release notes should get extracted.</param>
    /// <returns>A collection of the release notes.</returns>
    [Pure]
    public static (int insertIndex, List<string> content) ExtractChangelog(string changelogFile, string tag = null)
    {
        var content = TextTasks.ReadAllLines(changelogFile).ToList();

        var firstSectionIndex = content.FindIndex(x => x.StartsWith(tag ?? "## "));
        if (firstSectionIndex == -1)
        {
            return (content.Count, content);
        }

        return (firstSectionIndex, content);
    }

    public static string GenerateNewChangeLogSection(string version, string releaseTargetBranch, string headerPattern = null)
    {
        var sb = new StringBuilder();

        if (string.IsNullOrEmpty(headerPattern))
        {
            headerPattern = "## {0:yyyy-MM-dd} - {1}\n\n";
        }

        sb.AppendFormat(headerPattern, DateTime.Now, version);
        var entries = CollectLog(releaseTargetBranch)
                      .Where(e => !e.IsMergeCommit)
                      .GroupBy(e => e.Category)
                      .OrderBy(e => e.Key)
                      .ToList();
        if (entries.Count > 0)
        {
            var indentCount = entries.Select(e => e.Key.Length).Max() + 7;
            var space = new string(' ', indentCount);

            foreach (var x in entries)
            {
                foreach (var y in x.OrderBy(e => e.CommitTime))
                {
                    var x1 = indentCount - (6 + y.Category.Length);
                    sb.Append($"    [{y.Category}]");
                    sb.Append(new string(' ', x1));
                    sb.AppendLine(y.Subject);
                    sb.AppendLine();
                    if (y.Body.Count > 0)
                    {
                        foreach (var l in y.Body)
                        {
                            sb.Append(space);
                            sb.AppendLine(l);
                        }
                    }
                }
            }
        }

        return sb.ToString();
    }
    
    public static bool TryPrepareChangeLogForRelease(BuildState state, AbsolutePath changeLogFile, out string sectionFile)
    {
        if (!File.Exists(changeLogFile))
        {
            sectionFile = default;
            return false;
        }

        var (cl, section) = ChangeLogGenerator.UpdateChangeLogFile(changeLogFile, state.VersionTag, state.ReleaseTargetBranch);
        File.WriteAllText(changeLogFile, cl);
        sectionFile = Path.GetTempFileName();
        File.WriteAllText(sectionFile, section);
        return true;
    }


}
