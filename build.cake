var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var outputDirectory = "./artifacts";
var versionSuffix = "";

class ProjectInformation
{
    public string Name { get; set; }
    public string FullPath { get; set; }
    public bool IsTestProject { get; set; }
}

List<ProjectInformation> projects;

Setup(context =>
{
    projects = GetFiles("./src/**/*.csproj").Select(p => new ProjectInformation
    {
        Name = p.GetFilenameWithoutExtension().ToString(),
        FullPath = p.FullPath,
        IsTestProject = p.GetFilenameWithoutExtension().ToString().EndsWith(".Tests")
    }).ToList();
    
    if (BuildSystem.IsLocalBuild && string.IsNullOrEmpty(versionSuffix))
    {
        versionSuffix = "local";
    }
    
    if (configuration != "Release")
    {
        if (string.IsNullOrEmpty(versionSuffix))
        {
            versionSuffix += "-";
        }
        else
        {
            versionSuffix += ".";
        }
        versionSuffix += configuration.ToLowerInvariant();
    }
    
    Information($"Building with suffix {versionSuffix}");
});

Task("Clean")
    .Does(() => 
{
    var cleanSettings = new DotNetCoreCleanSettings { Configuration = configuration };
    foreach (var project in projects)
    {
        DotNetCoreClean(project.FullPath, cleanSettings);
    }
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var project in projects)
    {
        DotNetCoreRestore(project.FullPath);
    }
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var buildSettings = new DotNetCoreBuildSettings 
    { 
        Configuration = configuration,
        NoRestore = true
    };
    
    foreach (var project in projects)
    {
        DotNetCoreBuild(project.FullPath, buildSettings);
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    var testSettings = new DotNetCoreTestSettings 
    { 
        Configuration = configuration,
        NoBuild = true,
        NoRestore = true 
    };
    
    foreach (var testProject in projects.Where(p => p.IsTestProject))
    {
        DotNetCoreTest(testProject.FullPath, testSettings);
    }
});

Task("Pack")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
{
     var packSettings = new DotNetCorePackSettings
     {
         Configuration = configuration,
         OutputDirectory = outputDirectory,
         NoBuild = true,
         NoRestore = true,
         VersionSuffix = versionSuffix
     };
     
     foreach (var project in projects.Where(p => !p.IsTestProject))
     {
         DotNetCorePack(project.FullPath, packSettings);
     }
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
{
});

RunTarget(target);
