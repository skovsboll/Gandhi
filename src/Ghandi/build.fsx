// include Fake libs
#I @"..\packages\FAKE.1.64.8\tools"
#r "FakeLib.dll"
open Fake


// Default target
Target "Default" (fun _ ->
    trace "Hello World from FAKE"
    ExecProcess "csc.exe" 
)


// The clean target cleans the build and deploy folders
Target "Clean" (fun _ -> 
    CleanDirs ["./build/"; "./deploy/"]
)



// define test dlls
let testDlls = !! (testDir + @"/Test.*.dll")

Target "xUnitTest" (fun _ ->
    testDlls
        |> xUnit (fun p -> 
            {p with 
                ShadowCopy = false;
                HtmlOutput = true;
                XmlOutput = true;
                OutputDir = testDir })
)

// Dependencies
"Clean"  ==> "Default"

"Default" ==> "xUnitTest"

// start build
Run "Default"

