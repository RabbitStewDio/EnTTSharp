using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitVersion;
using System;

public class GitFlow
{
    readonly Build build;

    public GitFlow([NotNull] Build build)
    {
        this.build = build ?? throw new ArgumentNullException(nameof(build));
    }

    public event EventHandler<BuildType> UpdateVersionNumbers;

    public bool EnsureNoUncommittedChanges()
    {
        if (!GitTasks.GitHasCleanWorkingCopy())
            throw new Exception(
                "There are uncommitted changes in the working tree. Please commit or remove these before proceeding.");

        return true;
    }

    public void PrepareStagingBranch(BuildState state)
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        var versionInfo = FetchVersion();
        if (versionInfo.BranchName == state.ReleaseTargetBranch)
            throw new Exception(
                $@"Cannot initiate a release from the release-target branch. Switch to a develop or release-xxx branch before continuing. 

Based on the current version information I expect to be on branch '{state.DevelopmentBranch}', but detected branch '{versionInfo.BranchName}' instead");

        // if on development branch, create a release branch.
        // if you work in a support-xx branch, treat it as your develop-branch.
        var stageBranchName = state.ReleaseStagingBranch;
        if (versionInfo.BranchName == state.DevelopmentBranch)
        {
            if (GitTools.CheckBranchExists(stageBranchName))
            {
                Logger.Info($"Switching to existing staging branch from {versionInfo.BranchName} as branch {stageBranchName}");
                GitTools.Checkout(stageBranchName);
            }
            else
            {
                Logger.Info($"Creating new staging branch from current branch {versionInfo.BranchName} as branch {stageBranchName}");
                GitTools.Branch(stageBranchName);
                UpdateVersionNumbers?.Invoke(this, BuildType.Staging);
            }
        }
        else
        {
            if (versionInfo.BranchName != stageBranchName)
                throw new Exception(
                    $@"This command must be exist run from the development branch or an active release branch.
 
Based on the current version information I expect to be on branch '{state.ReleaseStagingBranch}', but detected branch '{versionInfo.BranchName}' instead");
        }
    }

    public void AttemptStagingBuild(BuildState state, Action<BuildType, string> action)
    {
        // We are now supposed to be on the release branch.
        EnsureOnReleaseStagingBranch(state);

        Logger.Info("Building current release as release candidate.");
        ValidateBuild(action, BuildType.Staging, null);
    }

    void ValidateBuild(Action<BuildType, string> runBuildTarget, BuildType t, string path)
    {
        if (runBuildTarget == null)
            throw new ArgumentException("RunBuildTarget action is not configured.");

        EnsureNoUncommittedChanges();

        Logger.Info("Running target build script.");
        runBuildTarget(t, path);

        Logger.Info("Restoring original assembly version files.");
        GitTools.Reset(GitTools.ResetType.Hard);
        EnsureNoUncommittedChanges();
    }


    public void PushStagingBranch()
    {
        var versionInfo = FetchVersion();
        var stageBranchName = string.Format(build.ReleaseStagingBranchPattern, versionInfo.MajorMinorPatch, versionInfo.Major, versionInfo.Minor, versionInfo.Patch);
        if (!string.IsNullOrEmpty(build.PushTarget))
        {
            Logger.Info("Publishing staging branch to public source repository.");
            GitTools.Push(build.PushTarget, stageBranchName);
        }
    }

    public bool EnsureOnReleaseStagingBranch(BuildState state)
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        Logger.Trace("Validating that the build is on the release branch.");
        var versionInfo = FetchVersion();
        if (versionInfo.BranchName != state.ReleaseStagingBranch)
            throw new Exception(
                "Not on the release branch. Based on the current version information I expect to be on branch '" +
                state.ReleaseStagingBranch + "', but detected branch '" + versionInfo.BranchName + "' instead");
        return true;
    }

    public void PerformRelease(BuildState state, AbsolutePath changeLogFile, Action<BuildType, string> buildAction)
    {
        var releaseId = Guid.NewGuid();
        var releaseBranchTag = "_release-state-" + releaseId;
        var stagingBranchTag = "_staging-state-" + releaseId;

        EnsureOnReleaseStagingBranch(state);

        GitTools.Tag(stagingBranchTag, state.ReleaseStagingBranch);

        try
        {
            if (ChangeLogGenerator.TryPrepareChangeLogForRelease(state, changeLogFile, out var sectionFile))
            {
                GitTools.Commit($"Updated change log for release {state.Version.MajorMinorPatch}");
            }


            // record the current master branch state.
            // We will use that later to potentially undo any changes we made during that build.
            GitTools.Tag(releaseBranchTag, state.ReleaseTargetBranch);

            try
            {
                // this takes the current staging branch state and merges it into
                // the release branch (usually master). This changes the active 
                // branch of the working copy.
                GitTools.MergeRelease(state.ReleaseTargetBranch, state.ReleaseStagingBranch);

                // attempt to build the release again.
                ValidateBuild(buildAction, BuildType.Release, sectionFile);
            }
            catch
            {
                Logger.Error("Error: Unable to build the release on the release branch. Attempting to roll back changes on release branch.");
                GitTools.Reset(GitTools.ResetType.Hard, releaseBranchTag);
                GitTools.Checkout(stagingBranchTag);
                GitTools.ResetBranch(state.ReleaseStagingBranch, stagingBranchTag);
                throw;
            }
            finally
            {
                GitTools.DeleteTag(releaseBranchTag);
            }
        }
        catch
        {
            // In case of errors, roll back all commits and restore the current state 
            // to be back on the release-staging branch.
            GitTools.Checkout(stagingBranchTag);
            GitTools.ResetBranch(state.ReleaseStagingBranch, stagingBranchTag);
            throw;
        }
        finally
        {
            GitTools.DeleteTag(stagingBranchTag);
        }
    }

    public void ContinueOnDevelopmentBranch(BuildState state)
    {
        EnsureNoUncommittedChanges();

        GitTools.Checkout(state.DevelopmentBranch);
        GitTools.Merge(state.ReleaseStagingBranch);

        UpdateVersionNumbers?.Invoke(this, BuildType.Development);
    }

    public BuildState RecordBuildState()
    {
        EnsureNoUncommittedChanges();

        return new BuildState(build.VersionTagPattern, build.DevelopBranch, build.ReleaseStagingBranchPattern, build.ReleaseTargetBranch);
    }


    const string GitVersionFramework = "net5.0";

    public static GitVersion FetchVersion()
    {
        return GitVersionTasks.GitVersion(s => s.SetFramework(GitVersionFramework)
                                                .EnableNoFetch()
                                                .DisableProcessLogOutput()
                                                .DisableUpdateAssemblyInfo())
                              .Result;
    }
}
