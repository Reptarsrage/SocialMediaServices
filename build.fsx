#I "./packages/FAKE/tools"
#r @"FakeLib.dll"
open System.IO
open System.Diagnostics
open System.Text.RegularExpressions
open Fake
open Fake.FileSystemHelper
open Fake.Testing

// Properties
// ==========
let fakePackageDir = "packages"
let artifactDir = "artifacts"
let outDir = "bin/Release/netcoreapp2.0"

// Functions
// =========
let packageProject (nuspecPath: string) = 
    let branch = getBuildParamOrDefault "branch" ""
    let branch =
        if branch = "master" || branch = "" then ""
        else "-" + Regex.Replace(branch, "[^a-zA-Z0-9]", "-")
    let dir = Path.GetDirectoryName(nuspecPath)
    let path = Path.Combine(dir, outDir, Path.GetFileNameWithoutExtension(nuspecPath) + ".dll")
    let version = FileVersionInfo.GetVersionInfo(path).FileVersion.ToString()

    System.Console.WriteLine("##teamcity[buildNumber '" + version + branch + "']")

    NuGetPack (fun p ->
        {p with 
            OutputPath = artifactDir
            WorkingDir = Path.GetDirectoryName(nuspecPath)
            Version = version + branch
            }) nuspecPath

// Targets
// =======
Target "RestorePackages" (fun _ ->
    let projects = !! ("**/*.csproj")
    for project in projects do
        DotNetCli.Restore
            (fun p -> 
                { p with 
                    Project = project
                    AdditionalArgs = [
                                     "-s https://api.nuget.org/v3/index.json" 
                                     ]})
)

Target "Clean" (fun _ ->
    CleanDir artifactDir
    Directory.GetDirectories(".", "bin", SearchOption.AllDirectories)
        |> Array.iter CleanDir
    Directory.GetDirectories(".", "obj", SearchOption.AllDirectories)
        |> Array.iter CleanDir

    ensureDirectory artifactDir
)

Target "BuildAll" (fun _ ->
    let projects = !! ("**/*.csproj")
    for project in projects do
        DotNetCli.Build
            (fun p -> 
                { p with 
                    Configuration = "Release"
                    Project = project})
)

Target "Test-Unit" (fun _ ->
    let projects = !! ("**/*.Tests.Unit.csproj")
    for project in projects do
        DotNetCli.Test
            (fun p -> 
                { p with 
                    Configuration = "Release"
                    Project = project})
)

Target "Package" (fun _ ->
    !! ("src/**/*.nuspec")
        -- "**/obj/**"
        |> Seq.iter packageProject
)

// Dependencies
// ============
"Clean"
  ==> "RestorePackages"

"RestorePackages"
  ==> "BuildAll"

"BuildAll"
  ==> "Test-Unit"

"Test-Unit"
  ==> "Package"

// Start
// =====
RunTargetOrDefault "Test-Unit"
