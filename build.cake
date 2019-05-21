// Kudos to Muhammed Rehan Saeed for this script!
// http://rehansaeed.com/cross-platform-devops-net-core/

// Target - The task you want to start. Runs the Default task if not specified.
var target = Argument("Target", "Default");
var configuration = Argument("Configuration", "Release");

Information("Running target " + target + " in configuration " + configuration);

// The build number to use in the version number of the built NuGet packages.
// There are multiple ways this value can be passed, this is a common pattern.
// 1. If command line parameter parameter passed, use that.
// 2. Otherwise if running on AppVeyor, get it's build number.
// 3. Otherwise if running on Travis CI, get it's build number.
// 4. Otherwise if an Environment variable exists, use that.
// 5. Otherwise default the build number to 0.
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 1;
 
// Assume we're building on appveyor for publishing NuGets
// So always add a beta prefix if not doing a tag
var isTag = EnvironmentVariable("APPVEYOR_REPO_TAG") != null && EnvironmentVariable("APPVEYOR_REPO_TAG") == "true" ;
var revision = isTag ? null : "beta-" + buildNumber.ToString("D4");
// A directory path to an Artifacts directory.
var artifactsDirectory = Directory("./artifacts");
 
// Deletes the contents of the Artifacts folder if it should contain anything from a previous build.
Task("Clean")
    .Does(() =>
    {
		DotNetCoreClean(".");
        CleanDirectory(artifactsDirectory);
    });
 
// Run dotnet restore to restore all package references.
Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });
 
// Find all sln files and build them using the build configuration specified as an argument.
 Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var solutions = GetFiles("./*.sln");
        foreach(var solution in solutions)
        {
            Information("Building solution " + solution);
            DotNetCoreBuild(
                solution.ToString(),
                new DotNetCoreBuildSettings()
                {
                    NoRestore = true,
                    Configuration = configuration,
                    VersionSuffix = revision,
                });
        }
    });
 
// Look under a 'Tests' folder and run dotnet test against all of those projects.
Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var projects = GetFiles("./test/**/*Tests.csproj");
        foreach(var project in projects)
        {
            Information("Testing project " + project);
            DotNetCoreTest(
                project.ToString(),
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true,
                    NoRestore = true,
                });
        }
    });
 
// The default task to run if none is explicitly specified. In this case, we want
// to run everything starting from Clean, all the way up to Pack.
Task("Default")
    .IsDependentOn("Test");
 
// Executes the task specified in the target argument.
RunTarget(target);