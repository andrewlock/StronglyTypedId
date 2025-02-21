using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath TestResultsDirectory => ArtifactsDirectory / "results";
    AbsolutePath OutputPackagesDirectory => ArtifactsDirectory / "packages";

    [Parameter] readonly string GithubToken;
    [Parameter] readonly string NuGetToken;
    [Parameter] readonly AbsolutePath PackagesDirectory = RootDirectory / "packages";

    const string NugetOrgUrl = "https://api.nuget.org/v3/index.json";
    bool IsTag => GitHubActions.Instance?.Ref?.StartsWith("refs/tags/") ?? false;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            PackagesDirectory.CreateOrCleanDirectory();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .When(_ => !string.IsNullOrEmpty(PackagesDirectory), x=>x.SetPackageDirectory(PackagesDirectory))
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .When(_ => IsServerBuild, x => x.SetProperty("ContinuousIntegrationBuild", "true"))
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .When(_ => IsServerBuild, x => x.SetProperty("ContinuousIntegrationBuild", "true"))
                .When(_ => IsServerBuild, x => x
                    .SetLoggers("trx")
                    .SetResultsDirectory(TestResultsDirectory))
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Produces(OutputPackagesDirectory)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetConfiguration(Configuration)
                .SetOutputDirectory(OutputPackagesDirectory)
                .When(_ => IsServerBuild, x => x.SetProperty("ContinuousIntegrationBuild", "true"))
                .EnableNoBuild()
                .EnableNoRestore()
                .SetProject(Solution));
        });

    Target TestPackages => _ => _
        .DependsOn(Pack)
        .After(Test)
        .Produces(OutputPackagesDirectory)
        .Executes(() =>
        {
            var projectFiles = new[]
            {
                TestsDirectory / "StronglyTypedIds.Nuget.IntegrationTests",
                TestsDirectory / "StronglyTypedIds.Nuget.Attributes.IntegrationTests",
            };

            if (!string.IsNullOrEmpty(PackagesDirectory))
            {
                (PackagesDirectory / "stronglytypedid").DeleteDirectory();
                (PackagesDirectory / "stronglytypedid.attributes").DeleteDirectory();
            }

            DotNetRestore(s => s
                .When(_ => !string.IsNullOrEmpty(PackagesDirectory), x => x.SetPackageDirectory(PackagesDirectory))
                .SetConfigFile(RootDirectory / "NuGet.integration-tests.config")
                .CombineWith(projectFiles, (s, p) => s.SetProjectFile(p)));

            DotNetBuild(s => s
                .When(_ => !string.IsNullOrEmpty(PackagesDirectory), x=>x.SetPackageDirectory(PackagesDirectory))
                .EnableNoRestore()
                .SetConfiguration(Configuration)
                .CombineWith(projectFiles, (s, p) => s.SetProjectFile(p)));

            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .When(_ => IsServerBuild, x => x
                    .SetLoggers("trx")
                    .SetResultsDirectory(TestResultsDirectory))
                .CombineWith(projectFiles, (s, p) => s.SetProjectFile(p)));

        });

    Target PushToNuGet => _ => _
        .DependsOn(Compile)
        .OnlyWhenStatic(() => IsTag && IsServerBuild && IsWin)
        .Requires(() => NuGetToken)
        .After(Pack)
        .Executes(() =>
        {
            var packages = OutputPackagesDirectory.GlobFiles("*.nupkg");
            DotNetNuGetPush(s => s
                .SetApiKey(NuGetToken)
                .SetSource(NugetOrgUrl)
                .EnableSkipDuplicate()
                .CombineWith(packages, (x, package) => x
                    .SetTargetPath(package)));
        });
}
