using Microsoft.Azure.KeyVault.Models;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Git;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using Action = System.Action;

partial class Build
{
    [Parameter("Develop branch - defaults to 'develop'")]
    public string DevelopBranch { get; set; }

    [Parameter("Release branch name pattern - defaults to 'release-{0}'")]
    public string ReleaseStagingBranchPattern { get; set; }

    [Parameter("Release merge target branch - The branch that receives the merged code after a successful build; defaults to 'master'")]
    public string ReleaseTargetBranch { get; set; }

    [Parameter("Version Tag name - a tag applied after the release has been built; defaults to 'v{0}'")]
    public string VersionTagPattern { get; set; }

    [Parameter("Git-Target used to push changes to the server - defaults to 'origin'")]
    public string PushTarget { get; set; }

    partial void OnGitFlowInitialized()
    {
        ParseConfig();
    }

    void ParseConfig()
    {
        try
        {
            var file = RootDirectory / "GitVersionBranches.yml";
            if (File.Exists(file))
            {
                var fileAsText = File.ReadAllText(file);
                var deserializer = new DeserializerBuilder().Build();
                var config = deserializer.Deserialize<GitVersionBranchModel>(fileAsText);
                ReleaseStagingBranchPattern ??= config.ReleaseStagingBranchPattern;
                VersionTagPattern ??= config.VersionTagPattern;
                ReleaseTargetBranch ??= config.ReleaseTargetBranch;
                DevelopBranch ??= config.DevelopBranch;
                PushTarget ??= config.PushTarget;
            }
        }
        catch
        {
            // its ok to fail.
        }

        ReleaseStagingBranchPattern ??= "release-{0}";
        VersionTagPattern ??= "v{0}";
        ReleaseTargetBranch ??= "master";
        DevelopBranch ??= "develop";
        PushTarget ??= "origin";
    }

    BuildState currentBuildState;

    BuildState GetOrCreateBuildState()
    {
        return currentBuildState ??= new BuildState(VersionTagPattern, DevelopBranch, ReleaseStagingBranchPattern, ReleaseTargetBranch);
    }
}
