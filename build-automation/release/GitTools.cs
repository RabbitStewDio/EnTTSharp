using Nuke.Common.Tooling;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities;
using System;

public static class GitTools
{
    public enum ResetType
    {
        Default = 0,
        Soft = 1,
        Hard = 2
    }

    public static void DeleteTag(string tagName)
    {
        GitTasks.Git($"tag -d {tagName.DoubleQuoteIfNeeded()}");
    }

    public static void Tag(string tagName, string branchOrCommitRef = null)
    {
        if (string.IsNullOrEmpty(branchOrCommitRef))
        {
            GitTasks.Git($"tag {tagName.DoubleQuoteIfNeeded()}");
        }
        else
        {
            GitTasks.Git($"tag {tagName.DoubleQuoteIfNeeded()} {branchOrCommitRef.DoubleQuoteIfNeeded()}");
        }
    }

    public static void TagAnnotated(string tagName, string branchOrCommitRef = null)
    {
        if (string.IsNullOrEmpty(branchOrCommitRef))
        {
            GitTasks.Git($"tag -a {tagName.DoubleQuoteIfNeeded()}");
        }
        else
        {
            GitTasks.Git($"tag -a {tagName.DoubleQuoteIfNeeded()} {branchOrCommitRef.DoubleQuoteIfNeeded()}");
        }
    }

    public static void CheckoutDetached(string branchName)
    {
        GitTasks.Git($"checkout --detach {branchName.DoubleQuoteIfNeeded()}");
    }

    public static void Checkout(string branchName, bool force = false)
    {
        var forceArg = force ? "-f" : ""; 
        GitTasks.Git($"checkout {forceArg} {branchName.DoubleQuoteIfNeeded()}");
    }

    public static void Branch(string branchName, bool force = false)
    {
        var forceArg = force ? "-f" : ""; 
        GitTasks.Git($"checkout -b {forceArg} {branchName.DoubleQuoteIfNeeded()}");
    }

    public static void ResetBranch(string branchName, string commitRef)
    {
        GitTasks.Git($"branch -f {branchName.DoubleQuoteIfNeeded()} {commitRef.DoubleQuoteIfNeeded()}");
    }

    public static void DeleteBranch(string branchName, bool force = false)
    {
        var forceArg = force ? "-f" : ""; 
        GitTasks.Git($"branch -d {forceArg} {branchName.DoubleQuoteIfNeeded()}");
    }

    public static void Merge(string branchName, bool allowFF = false)
    {
        var disableFastForwardArg = allowFF ? "" : "--no-ff"; 
        GitTasks.Git($"merge {disableFastForwardArg} {branchName.DoubleQuoteIfNeeded()}");
    }

    public static void MergeOurs(string branchName)
    {
        GitTasks.Git($"merge -s ours {branchName.DoubleQuoteIfNeeded()}");
    }

    /// A merge that forces the contents of the release-branch into master while preserving master as a series of merges. This never fails.
    /// This is also a safe operation (in our git-flow context here), as master is the representation of releases and is fed from release-branches.
    /// Those release branches are simultaneously merged both into master and develop.
    /// 
    /// This merge script is based on the answer in StackOverflow here:
    /// http://stackoverflow.com/a/27338013
    public static void MergeRelease(string releaseBranch, string stageBranchName)
    {
        Checkout(releaseBranch, true);

        // Do a merge commit. The content of this commit does not matter, so use a strategy that never fails.
        // Note: This advances branchA.
        MergeOurs(stageBranchName);

        // # Change working tree and index to desired content.
        // # --detach ensures branchB will not move when doing the reset in the next step.
        CheckoutDetached(stageBranchName);

        // # Move HEAD to branchA without changing contents of working tree and index.
        Reset(ResetType.Soft, releaseBranch);

        // # 'attach' HEAD to branchA. # This ensures branchA will move when doing 'commit --amend'.
        Checkout(releaseBranch, false);

        // # Change content of merge commit to current index (i.e. content of branchB).
        CommitAmend();
    }

    public static void Push(string remoteName, string branchName)
    {
        GitTasks.Git($"push {remoteName.DoubleQuoteIfNeeded()} {branchName.DoubleQuoteIfNeeded()}");
    }

    public static void PushTag(string remoteName, string tagName)
    {
        GitTasks.Git($"push --tags {remoteName.DoubleQuoteIfNeeded()} {tagName.DoubleQuoteIfNeeded()}");
    }

    public static void CommitAmend()
    {
        GitTasks.Git($"commit --amend -C HEAD");
    }

    public static void Commit(string message, bool all = false)
    {
        GitTasks.Git($"commit -a -m {message.DoubleQuoteIfNeeded()}");
    }

    public static void Add(string file)
    {
        GitTasks.Git($"add {file.DoubleQuoteIfNeeded()}");
    }

    public static void Reset(ResetType type = ResetType.Default, string id = null)
    {
        var resetTypeStr = type switch
        {
            ResetType.Default => "",
            ResetType.Soft => "--soft",
            ResetType.Hard => "--hard",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        GitTasks.Git($"reset {resetTypeStr} {id.DoubleQuoteIfNeeded()}");
    }

    public static bool CheckUncommittedChanges()
    {
        return GitTasks.GitHasCleanWorkingCopy();
    }

    public static bool CheckBranchExists(string branchName)
    {
        try
        {
            return GitTasks.Git($"show-ref --heads {branchName.DoubleQuoteIfNeeded()}").Count > 0;
        }
        catch (ProcessException pe)
        {
            return false;
        }
    }
    
}
