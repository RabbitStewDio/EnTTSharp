using JetBrains.Annotations;
using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using System.IO;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
public partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Release);

    public event Action BuildInitialized;

    partial void OnGitFlowInitialized();

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;

    AbsolutePath ChangeLogFile => RootDirectory / "CHANGELOG.md";

    [Parameter("Build Tool Parameters")]
    readonly string BuildToolParameters;

    [Solution]
    readonly Solution Solution;

    [GitRepository]
    [UsedImplicitly]
    readonly GitRepository GitRepository;

    readonly GitFlow GitFlow;

    [LocalExecutable("./build.ps1")]
    Tool BuildScript;

    public Build()
    {
        GitFlow = new GitFlow(this);
    }

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();
        BuildInitialized?.Invoke();
        OnGitFlowInitialized();

        if (IsWin)
        {
            BuildScript = ToolResolver.GetLocalTool(RootDirectory / "build.cmd");
        }
        else
        {
            BuildScript = ToolResolver.GetLocalTool(RootDirectory / "build.sh");
        }
    }


    AbsolutePath ArtifactsDirectory => RootDirectory / "output";
    AbsolutePath ArtifactsArchiveDirectory => RootDirectory / "build-artefacts";

    Target Clean => _ =>
        _.Description("Removes all previous build artefacts from the archive directory")
         .Executes(() =>
         {
             RootDirectory.GlobDirectories("bin", "obj").ForEach(DeleteDirectory);
             EnsureCleanDirectory(ArtifactsDirectory);
             EnsureCleanDirectory(ArtifactsArchiveDirectory);
         });


    Target PrepareRelease => _ =>
        _.Description("Switches the repository to the current release staging branch - or creates one if needed.")
         .Executes(() =>
         {
             GitFlow.EnsureNoUncommittedChanges();
             
             var state = GetOrCreateBuildState();
             GitFlow.PrepareStagingBranch(state);
         });

    Target BuildStagingBuild => _ =>
        _.Description("Invokes the builds script for a staging build to validate that the build passes and all tests run without errors. This is a pre-requisite for a release.")
         .DependsOn(PrepareRelease)
         .Executes(() =>
         {
             GitFlow.EnsureNoUncommittedChanges();
             GitFlow.EnsureOnReleaseStagingBranch(GetOrCreateBuildState());
             
             var state = GetOrCreateBuildState();
             GitFlow.AttemptStagingBuild(state, PerformBuild);
         });


    Target Release => _ =>
        _.DependsOn(BuildStagingBuild)
         .Executes(() =>
         {
             GitFlow.PerformRelease(GetOrCreateBuildState(), ChangeLogFile, PerformBuild);
         });

    Target ContinueDevelopment => _ =>
        _.Executes(() =>
        {
            GitFlow.EnsureNoUncommittedChanges();
            
            var state = GetOrCreateBuildState();
            GitFlow.ContinueOnDevelopmentBranch(state);
        });
    


    Target UpdateChangeLog => _ =>
        _.Executes(() =>
        {
            var state = GetOrCreateBuildState();
            var (cl, section) = ChangeLogGenerator.UpdateChangeLogFile(ChangeLogFile, state.VersionTag, state.ReleaseTargetBranch);
            File.WriteAllText(ChangeLogFile, cl);
        });


    void PerformBuild(BuildType type, string changeLogSection = null)
    {
        if (changeLogSection != null)
        {
            BuildScript($"default --configuration {Configuration} --package-release-notes-file {changeLogSection.DoubleQuoteIfNeeded()} {BuildToolParameters}");
            if (type == BuildType.Release)
            {
                BuildScript($"upload --configuration {Configuration} --package-release-notes-file {changeLogSection.DoubleQuoteIfNeeded()} {BuildToolParameters}");
            }
        }
        else
        {
            BuildScript($"default --configuration {Configuration} {BuildToolParameters}");
        }
    }
}