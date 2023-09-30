using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.GitHubTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[SuppressMessage("ReSharper", "InconsistentNaming")]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Default);

    static readonly string[] DefaultPublishTargets =
    {
        "win-x64",
//        "linux-x64"
    };

    const string NuGetDefaultUrl = "https://api.nuget.org/v3/index.json";

    [Parameter("NuGet Source - Defaults to the value of the environment variable 'NUGET_SOURCE' or '" + NuGetDefaultUrl + "'")]
    string NuGetSource;

    [Parameter("NuGet API Key - Defaults to the value of the environment variable 'NUGET_API_KEY'")]
    string NuGetApiKey;

    [Parameter("Publish Target Runtime Identifiers for building self-contained applications - Default is ['win-x64', 'linux-x64']")]
    string[] PublishTargets = DefaultPublishTargets;

    [Parameter("Skip Tests - Default is 'false'")]
    readonly bool SkipTests;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("(Optional) GitHub Authentication Token for uploading releases - Defaults to the value of the GITHUB_API_TOKEN environment variable")]
    string GitHubAuthenticationToken;

    [Parameter("(Optional) Git remote id override")]
    string GitRepositoryRemoteId;

    [Parameter("(Optional) Path to the release notes for NuGet packages and GitHub releases.")]
    AbsolutePath PackageReleaseNotesFile;

    [Solution]
    readonly Solution Solution;

    GitRepository GitRepository;

    [GitVersion(NoFetch = true, Framework = "net5.0")]
    readonly GitVersion GitVersion;

    string ChangeLogFile => RootDirectory / "CHANGELOG.md";

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath SamplesDirectory => RootDirectory / "samples";

    AbsolutePath ArtifactsDirectory => RootDirectory / "output";
    AbsolutePath ArtifactsArchiveDirectory => RootDirectory / "build-artefacts";
    AbsolutePath NuGetTargetDirectory => ArtifactsArchiveDirectory / GitVersion.SemVer / "nuget";

    public Build()
    { }

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();

        if (string.IsNullOrEmpty(GitRepositoryRemoteId))
        {
            GitRepositoryRemoteId = "origin";
        }

        GitRepository = GitRepository.FromLocalDirectory(RootDirectory, null, GitRepositoryRemoteId);

        PublishTargets ??= DefaultPublishTargets;
        NuGetApiKey ??= Environment.GetEnvironmentVariable("NUGET_API_KEY");
        NuGetSource ??= Environment.GetEnvironmentVariable("NUGET_SOURCE") ?? NuGetDefaultUrl;
        GitHubAuthenticationToken ??= Environment.GetEnvironmentVariable("GITHUB_API_TOKEN");
    }

    Target Clean => _ => _
                         .Before(Restore)
                         .Executes(() =>
                         {
                             EnsureCleanDirectory(ArtifactsDirectory);
                             DotNetClean(s => s.SetProject(Solution)
                                               .SetConfiguration(Configuration)
                                               .SetAssemblyVersion(GitVersion.AssemblySemVer)
                                               .SetFileVersion(GitVersion.AssemblySemFileVer)
                                               .SetInformationalVersion(GitVersion.InformationalVersion));
                         });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ =>
        _
            .DependsOn(Restore)
            .Executes(() =>
            {
                Logger.Info($"Building with version {GitVersion.AssemblySemFileVer}");
                DotNetBuild(s => s
                                 .SetProjectFile(Solution)
                                 .SetConfiguration(Configuration)
                                 .SetAssemblyVersion(GitVersion.AssemblySemVer)
                                 .SetFileVersion(GitVersion.AssemblySemFileVer)
                                 .SetInformationalVersion(GitVersion.InformationalVersion)
                                 .EnableNoRestore());
            });

    Target Test => _ =>
        _
            .DependsOn(Compile)
            .Executes(() =>
            {
                if (SkipTests)
                {
                    return;
                }

                DotNetTest(s => s.SetProjectFile(Solution)
                                 .SetConfiguration(Configuration)
                                 .EnableNoRestore());
            });

    Target Pack => _ =>
        _.DependsOn(Test)
         .Executes(() =>
         {
             string releaseNotes = null;
             // Seriously, I have no clue what the format of those release notes should
             // be and how NuGet.org actually formats that stuff. It seems either broken
             // or random and most packages do not ship release notes or provide
             // a single link to an external website (aka its broken and I work around
             // it
             //
             // So uncomment the line below at your own risk.
             
             /*
             if (!string.IsNullOrEmpty(PackageReleaseNotesFile) && File.Exists(PackageReleaseNotesFile))
             {
                 releaseNotes = TextTasks.ReadAllText(PackageReleaseNotesFile);
             }
             */

             DotNetPack(s => s.SetProject(Solution)
                              .SetConfiguration(Configuration)
                              .SetVersion(GitVersion.NuGetVersionV2)
                              .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
                              .EnableIncludeSymbols()
                              .EnableIncludeSource()
                              .EnableNoRestore()
                              .When(!string.IsNullOrEmpty(releaseNotes), x => x.SetPackageReleaseNotes(releaseNotes)));

             GlobFiles(ArtifactsDirectory, "**/*.nupkg")
                 .NotEmpty()
                 .ForEach(absPath =>
                 {
                     Logger.Info(absPath);
                     var fileName = Path.GetFileName(absPath);
                     CopyFile(absPath, NuGetTargetDirectory / fileName, FileExistsPolicy.OverwriteIfNewer);
                 });

             GlobFiles(ArtifactsDirectory, "**/*.snupkg")
                 .NotEmpty()
                 .ForEach(absPath =>
                 {
                     Logger.Info(absPath);
                     var fileName = Path.GetFileName(absPath);
                     CopyFile(absPath, NuGetTargetDirectory / fileName, FileExistsPolicy.OverwriteIfNewer);
                 });
         });

    Target Publish => _ =>
        _.DependsOn(Test)
         .Executes(() =>
         {
             var projects = GlobFiles(SourceDirectory, "**/*.csproj")
                 .Concat(GlobFiles(SamplesDirectory, "**/*.csproj"));

             foreach (var target in PublishTargets)
             {
                 var runtimeValue = target;
                 foreach (var projectFile in projects)
                 {
                     Logger.Info("Processing " + projectFile);

                     // parsing MSBuild projects is pretty much broken thanks to some 
                     // buggy behaviour with MSBuild itself. For some reason MSBuild
                     // cannot load its own runtime and then simply crashes wildly
                     //
                     // https://github.com/dotnet/msbuild/issues/5706#issuecomment-687625355
                     // https://github.com/microsoft/MSBuildLocator/pull/79
                     //
                     // That issue is still active.

                     var project = new ProjectParser().Parse(projectFile, Configuration, runtimeValue);
                     if (project.OutputType != "WinExe" &&
                         project.OutputType != "Exe")
                     {
                         continue;
                     }


                     if (!project.IsValidPlatformFor(runtimeValue))
                     {
                         continue;
                     }

                     foreach (var framework in project.TargetFrameworks)
                     {
                         Logger.Info($"Processing {project.Name} with runtime {runtimeValue}");

                         DotNetPublish(s => s.SetProject(projectFile)
                                             .SetAssemblyVersion(GitVersion.AssemblySemVer)
                                             .SetFileVersion(GitVersion.AssemblySemFileVer)
                                             .SetInformationalVersion(GitVersion.InformationalVersion)
                                             .SetRuntime(runtimeValue)
                                             .EnableSelfContained());

                         var buildTargetDir = ArtifactsDirectory / project.Name / "bin" / Configuration / framework;
                         var archiveTargetDir = ArtifactsArchiveDirectory / GitVersion.SemVer / "builds" / runtimeValue / framework;
                         var archiveFile = archiveTargetDir / $"{project.Name}-{GitVersion.SemVer}.zip";

                         DeleteFile(archiveFile);
                         CompressZip(buildTargetDir, archiveFile);
                     }
                 }
             }
         });

    Target PushNuGet => _ =>
        _.Description("Uploads all generated NuGet files to the configured NuGet server")
         .OnlyWhenDynamic(() => !string.IsNullOrEmpty(NuGetSource))
         .OnlyWhenDynamic(() => !string.IsNullOrEmpty(NuGetApiKey))
         .Executes(() =>
         {
             GlobFiles(NuGetTargetDirectory, "*.nupkg")
                 .Where(x => !x.EndsWith(".symbols.nupkg"))
                 .ForEach(x =>
                 {
                     DotNetNuGetPush(s => s.SetTargetPath(x)
                                           .EnableSkipDuplicate()
                                           .EnableNoServiceEndpoint()
                                           .SetSource(NuGetSource)
                                           .SetApiKey(NuGetApiKey));
                 });
         });

    Target PublishGitHubRelease => _ =>
        _.Description("Uploads all generated package files to GitHub")
         .OnlyWhenDynamic(() => !string.IsNullOrEmpty(GitHubAuthenticationToken))
         .OnlyWhenDynamic(() => IsGitHubRepository())
         .Executes(() =>
         {
             var releaseTag = $"v{GitVersion.MajorMinorPatch}";

             string releaseNotes = null;
             if (!string.IsNullOrEmpty(PackageReleaseNotesFile) && File.Exists(PackageReleaseNotesFile))
             {
                 releaseNotes = TextTasks.ReadAllText(PackageReleaseNotesFile);
             }

             var repositoryInfo = GetGitHubRepositoryInfo(GitRepository);
             var releaseArtefacts = GlobFiles(NuGetTargetDirectory, "*.nupkg")
                                    .Concat(GlobFiles(NuGetTargetDirectory, "*.snupkg"))
                                    .Concat(GlobFiles(ArtifactsArchiveDirectory / GitVersion.SemVer / "builds", "**/*.zip"))
                                    .ToArray();
             

             Logger.Info($"Publishing {releaseArtefacts.Length} artefacts");
             PublishRelease(x => x.SetArtifactPaths(releaseArtefacts)
                                        .SetCommitSha(GitVersion.Sha)
                                        .SetReleaseNotes(releaseNotes)
                                        .SetRepositoryName(repositoryInfo.repositoryName)
                                        .SetRepositoryOwner(repositoryInfo.gitHubOwner)
                                        .SetTag(releaseTag)
                                        .SetToken(GitHubAuthenticationToken)).GetAwaiter().GetResult();
         });

    bool IsGitHubRepository()
    {
        return string.Equals(GitRepository?.Endpoint, "github.com");
    }

    Target Default => _ =>
        _.Description("Builds the project and produces all release artefacts")
         .DependsOn(Clean)
         .DependsOn(Publish)
         .DependsOn(Pack);

    Target Upload => _ =>
        _.Description("Uploads all generated release artefacts to the NuGet repository and GitHub")
         .DependsOn(PushNuGet)
         .DependsOn(PublishGitHubRelease);
}
