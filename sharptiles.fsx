// include Fake lib
#r "tools/FAKE/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open System
open System.IO
open System.Text.RegularExpressions
open System.Collections


RestorePackages()

// Properties
let frameworkVersion = "v4.0.30319"
let buildDir = "./build/"
let buildTempDir = "./build/temp"
let releaseDir = "./distribution"
let testDir  = "./build/tests/"
let buildMode = getBuildParamOrDefault "buildMode" "Release"

let setParams defaults =
        { defaults with
            Verbosity = Some(Quiet)
            Targets = ["Build"]
            Properties =
                [
                    "Optimize", "True"
                    "DebugSymbols", "True"
                    "Configuration", buildMode
                ]
         }


let Compile(solutionFile: string) (configuration: string) =
    !! solutionFile
      |> MSBuild buildDir "Build" [("Configuration", configuration); ("PlatformTarget", "AnyCpu")]
      |> Log ("Compile-Output "+solutionFile+": ")

let MergeAssemblies (dllNames: string list) =
    let prefixedDllNames = query {
      for dllName in dllNames do
      select (releaseDir+"\\"+dllName)
    }

    let dllNamesArg = String.Join(" ",prefixedDllNames)
  
    let result = Shell.Exec("tools/ILMerge/ILMerge.exe", "/v4  /ndebug /target:library /out:"+releaseDir+"\\org.SharpTiles.dll /lib:library "+dllNamesArg)
    if result <> 0 then failwithf "ILMerge failed for %s based on %s\n" "org.SharpTiles.dll" dllNamesArg 

let RunUnit3 (dllName: string) =
    let result = Shell.Exec("tools/nunit3/nunit-console", "--labels=All --exclude:Performance --workers=1 --out=build/tests/"+dllName+".result.xml build/tests/"+dllName+"/"+dllName+".dll")
    if result <> 0 then failwithf "nunit failed for %s\n" dllName
    //if result <> 0 then printf "nunit failed for %s\n" dllName

let PrepareAndRunUnit3 (dllName: string, includeDocumentation: bool) =
    let targetDir = testDir+"/"+dllName
    let buildResourceNlDir = buildDir+"/nl-NL"
    let targetResourceNlDir = targetDir+"/nl-NL"
    CopyDir targetDir ("SharpTiles/"+dllName) (fun (f: string) -> 
      let extension = toLower (ext f)
      match extension with
      | ".html" -> true
      | ".htm" -> true
      | ".xsl" -> true
      | ".xml" -> true
      | ".txt" -> true
      | ".resources" -> true
      | ".resx" -> true
      | _ ->  
          trace f 
          false
    )
    !! "library/*.dll"
    |>  Copy targetDir
    !! (releaseDir+"/*.dll")
    |>  Copy targetDir
    
    if includeDocumentation then 
      !! (releaseDir+"/*.exe")
      |>  Copy targetDir
    if includeDocumentation then
      !! (buildDir+"/templates/*")
      |>  Copy (targetDir+"/templates")
      
    if TestFile (buildResourceNlDir+"/"+dllName+".resources.dll") then Copy targetResourceNlDir [buildResourceNlDir+"/"+dllName+".resources.dll"]
    Copy targetDir [buildDir+"/"+dllName+".dll"] 
    RunUnit3 dllName

let BuildSolutionForTarget (target: String)  = 
   Compile "SharpTiles/SharpTiles.sln" target 
   Copy releaseDir [buildDir+"org.SharpTiles.Common.dll"] 
   Copy releaseDir [buildDir+"org.SharpTiles.Connectors.dll"] 
   Copy releaseDir [buildDir+"org.SharpTiles.Documentation.exe"] 
   Copy releaseDir [buildDir+"org.SharpTiles.Expressions.dll"] 
   Copy releaseDir [buildDir+"org.SharpTiles.HtmlTags.dll"] 
   Copy releaseDir [buildDir+"org.SharpTiles.Tags.dll"] 
   Copy releaseDir [buildDir+"org.SharpTiles.Templates.dll"] 
   Copy releaseDir [buildDir+"org.SharpTiles.Tiles.dll"] 
   Copy releaseDir [buildDir+"org.SharpTiles.NUnit.dll"] 
   MergeAssemblies ["org.SharpTiles.Common.dll"; "org.SharpTiles.Expressions.dll"; "org.SharpTiles.HtmlTags.dll"; "org.SharpTiles.Tags.dll"; "org.SharpTiles.Templates.dll"; "org.SharpTiles.Tiles.dll"]

   
// Targets
Target "Clean" (fun _ ->
    CleanDirs [testDir;buildDir;releaseDir] 
)

Target "CreateTestDir" (fun _ ->
     CreateDir testDir
)


Target "BuildSolution" (fun _ ->
   BuildSolutionForTarget "Debug"
)

Target "BuildSolution4Release" (fun _ ->
   BuildSolutionForTarget "Release"
)

Target "CopyLibs" ( fun _ ->
  !! "library/*.dll"
    |>  Copy buildDir
)

Target "Default" (fun _ ->
    trace "SharpTiles build with FAKE"
)

Target "Test" (fun _ ->
    PrepareAndRunUnit3 ("org.SharpTiles.Common.Test", false)
    PrepareAndRunUnit3 ("org.SharpTiles.Expressions.Test", false)
    PrepareAndRunUnit3 ("org.SharpTiles.Tags.Test", false)
    PrepareAndRunUnit3 ("org.SharpTiles.HtmlTags.Test", false)
    PrepareAndRunUnit3 ("org.SharpTiles.Templates.Test", false)
    PrepareAndRunUnit3 ("org.SharpTiles.Tiles.Test", false)
    PrepareAndRunUnit3 ("org.SharpTiles.Documentation.Test", true)
     
    trace "SharpTiles tests with FAKE"
)

Target "InitReleaseDir" (fun _ ->
  CreateDir releaseDir
)



Target "BuildCode" (fun _ ->
  Run "Clean"  
  Run "BuildSolution"
  Run "CopyLibs" 
)

//"BuildSolution4Release"
//  ==> "CopyLibs"
//  ==> "BuildRelease"
Target "BuildRelease" (fun _ ->
  Run "BuildSolution4Release"
  Run "CopyLibs"
)

Target "BuildReleaseCode" (fun _ ->
  Run "Clean"  
  Run "InitReleaseDir"
  Run "BuildRelease"
)



"BuildCode"
  ==> "Test"
  ==> "Default"

// Dependencies
"CreateTestDir"
  ==> "Test"

Target "Release" (fun _ ->
Run "Clean" 
Run "InitReleaseDir"
Run "BuildReleaseCode"
Run "Test"
)


// start build
RunTargetOrDefault "Default"
