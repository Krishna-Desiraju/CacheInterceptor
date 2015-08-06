// include Fake lib
#I @"packages\FAKE\tools"
#I @"C:\FAKE\tools"

#r @"FakeLib.dll"

open Fake
open Fake.VSTest
open Fake.XDTHelper
open Fake.ReportGeneratorHelper
open Fake.AssemblyInfoFile
open Fake.XDTHelper

open System
open System.IO

// Properties
let sln                   = "./CacheInterceptor.sln"
let buildMode             = "Release"
let platform              = "Any CPU"
let majorVersion          = "1"
let minorVersion          = "0"

let msBuildProperties = 
    [ ("Configuration", buildMode)
      ("Platform", platform)
//      ("DeployOnBuild", "true")
//      ("PublishProfile", "ReleaseArtifacts.pubxml")
      ]

let testResultsDir = "./TestResults/"
let releaseArtifactsDir = "./ReleaseArtifacts/"
let releaseArtifactsNugetLibDir = "./ReleaseArtifacts/lib/net45"

// Fake Deploy Nuget Package Related Parameters
let packagingDir = "./PackagingDir/"
let libProjectName = "CacheInterceptor"
let nuspecFile = "CacheInterceptor.nuspec"
let authors = ["Krishna.Desiraju" ]
let packageName = "CacheInterceptor"
let description = "CacheInterceptor - Castle.Core Interceptor to cache method calls for any specified duration"
let tags = "cache interceptor aop proxy"

trace "------------Fake - Build Started-------------"

let machineEnv = getMachineEnvironment()
trace "Machine Details"
trace ("MachineName         : " + machineEnv.MachineName)
trace ("OperatingSystem     : " + machineEnv.OperatingSystem)
trace ("Is64bit             : " + machineEnv.Is64bit.ToString())
trace ("ProcessorCount      : " + machineEnv.ProcessorCount.ToString())
trace ("UserDomainName      : " + machineEnv.UserDomainName)
trace ("NETFrameworks       : " + (machineEnv.NETFrameworks |> Seq.reduce (fun a b -> a + "; " + b )) )
trace ("DriveInfo           : " + (machineEnv.DriveInfo |> Seq.reduce (fun a b -> a + "; " + b )))
trace ("Fake AgentVersion   : " + machineEnv.AgentVersion )

let mutable version = 
    majorVersion + "." + minorVersion + "." + match BuildServerHelper.isLocalBuild with
                                              | true -> "0"
                                              | false -> BuildServerHelper.buildVersion.ToString()

// Updates all AssemblyInfo files with the generated version number 
Target "UpdateAssemblyInfo" (fun _ -> 
    tracefn "Updating AssemblyInfo files with version number %s" version
    Fake.AssemblyInfoHelper.BulkReplaceAssemblyInfoVersions "./" (fun f -> 
        { f with AssemblyVersion = version
                 AssemblyFileVersion = version }))

// Clean
Target "Clean" (fun _ ->
    CleanDirs [ releaseArtifactsDir ; testResultsDir ; packagingDir]
    MSBuild null "Clean" msBuildProperties [ sln ] |> ignore
    RestorePackages()
)

// Compile
Target "Compile" (fun _ -> 
    MSBuild null "Build" msBuildProperties [ sln ] |> ignore
)

// Undo Updates all AssemblyInfo files with the generated version number 
Target "UndoUpdateAssemblyInfo" (fun _ -> 
    Fake.AssemblyInfoHelper.BulkReplaceAssemblyInfoVersions "./" (fun f -> 
        { f with AssemblyVersion = "1.0.0.0"
                 AssemblyFileVersion = "1.0.0.0" }))

// Run Unit Tests
Target "RunUnitTests" (fun _ -> 
    Directory.GetFiles("./", "*Test*.dll", SearchOption.AllDirectories)
    |> Array.filter 
           (fun file -> 
           let fileDetail = fileInfo file
           fileDetail.DirectoryName.Contains(fileNameWithoutExt fileDetail.Name) 
           && fileDetail.DirectoryName.Contains(@"\bin\") && fileDetail.DirectoryName.Contains(buildMode))
    |> VSTest(fun p -> 
           { p with EnableCodeCoverage = true
                    InIsolation = true
                    TestCaseFilter = "TestCategory=Unit"
                    Logger = "trx"
                    })
)

Target "CreateReleaseArtifacts" (fun _ ->
    CleanDir releaseArtifactsNugetLibDir
    
    // get bin dir in the project folder
    let getBinDirForApp appName appsDir platform buildMode =
        let platformDir platform= 
            match platform with
            | "Any CPU" -> ""
            | _ -> platform
        appsDir @@ appName @@ (@"\bin\") @@ (platformDir platform) @@ buildMode

    // find the location of binaries
    let binDir = getBinDirForApp libProjectName "./" platform buildMode
    tracefn "Bin directory for %s app is %s" libProjectName binDir

    !! (binDir @@ "/CacheInterceptor.*")
        |> Seq.iter (fun file -> CopyFile releaseArtifactsNugetLibDir file)
)

Target "CreateNugetDeploymentPackages" (fun _ -> 
    CleanDirs [packagingDir]
    !!(releaseArtifactsDir @@ "/" + packageName + "*.nupkg") 
        |> Seq.iter (fun x -> 
                tracefn "Deleting %s" x
                File.Delete(x))

    CopyDir 
        packagingDir
        releaseArtifactsDir 
        (fun _ -> true)
    
    nuspecFile |> NuGet(fun p -> 
                               { p with Authors = authors
                                        Project = packageName
                                        Version = version
                                        NoPackageAnalysis = true
                                        Description = description
                                        Tags = tags
                                        WorkingDir = packagingDir
                                        Dependencies = ["Castle.Core", GetPackageVersion "./packages/" "Castle.Core"]
                                        Publish = false
                                        OutputPath = releaseArtifactsDir})
)

// Default target
Target "Default" (fun _ ->
    trace "------------Fake - Build Completed-------------"
)

// Dependencies
"Clean"
    ==> "UpdateAssemblyInfo"
    ==> "Compile"
    ==> "UndoUpdateAssemblyInfo"
    ==> "RunUnitTests"
    ==> "CreateReleaseArtifacts"
    ==> "CreateNugetDeploymentPackages"
    ==> "Default"

// start build
RunTargetOrDefault "Default"