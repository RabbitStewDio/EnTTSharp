using Nuke.Common;
using Nuke.Common.Tools.GitVersion;
using System;

public class BuildState
{
    readonly string versionTagPattern;
    readonly string developBranch;
    readonly string stagingBranchPattern;
    readonly string targetBranchPattern;

    public BuildState(string versionTagPattern,
                      string developBranch,
                      string stagingBranchPattern,
                      string releaseTargetBranch)
    {
        this.versionTagPattern = versionTagPattern;
        this.developBranch = developBranch;
        this.stagingBranchPattern = stagingBranchPattern;
        this.targetBranchPattern = releaseTargetBranch;
        Version = GitFlow.FetchVersion();
        StartingBranch = Version.BranchName;
    }

    public GitVersion Version { get; }

    public string StartingBranch { get; }

    public string DevelopmentBranch => developBranch;
    
    public string VersionTag => string.Format(versionTagPattern, Version.MajorMinorPatch, Version.Major, Version.Minor, Version.Patch);

    public string ReleaseStagingBranch => string.Format(stagingBranchPattern, Version.MajorMinorPatch, Version.Major, Version.Minor, Version.Patch);
    public string ReleaseTargetBranch => string.Format(targetBranchPattern, Version.MajorMinorPatch, Version.Major, Version.Minor, Version.Patch);
       
    /// <summary>
    ///  These properties are only valid during a build.
    /// </summary>
    public bool IsStagingBuild
    {
        get
        {
            Logger.Normal("Validating that the build is on the release branch.");
            var versionInfo = GitFlow.FetchVersion();
            return versionInfo.BranchName == ReleaseStagingBranch;
        }
    }

    /// <summary>
    ///  These properties are only valid during a build.
    /// </summary>
    public bool IsReleaseBuild
    {
        get
        {
            Logger.Normal("Validating that the build is on the release branch.");
            var versionInfo = GitFlow.FetchVersion();
            return versionInfo.BranchName == ReleaseTargetBranch;
        }
    }
        
    public void ShowVersion()
    {
        var versionInfo = GitFlow.FetchVersion();
        Logger.Info("Computed Version: " + versionInfo.FullSemVer);
        Logger.Info("- Current branch: " + StartingBranch);
        Logger.Info("- Develop branch: " + developBranch);
        Logger.Info("- Staging-Branch: " + ReleaseStagingBranch);
        Logger.Info("- Target-Branch: " + ReleaseTargetBranch);
    }
        
    public void ValidateBuildState()
    {
        GitTools.CheckBranchExists(developBranch);
        GitTools.CheckBranchExists(ReleaseTargetBranch);
    }

}
