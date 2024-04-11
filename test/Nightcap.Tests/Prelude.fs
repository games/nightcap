module Nightcap.Tests.Prelude


open System
open Xunit
open Swensen.Unquote
open Nightcap


[<Fact>]
let ``envVars should works`` () =
    envVars "envVarsTest" =! None

    Environment.SetEnvironmentVariable("envVarsTest", "test")
    envVars "envVarsTest" =! Some "test"


[<Fact>]
let ``requiredEnv should works`` () =
    raises <@ requiredEnv "requiredEnvTest" = "" @>

    Environment.SetEnvironmentVariable("requiredEnvTest", "test")
    requiredEnv "requiredEnvTest" = "test"


[<Fact>]
let ``envExists should works`` () =
    envExists "envExistsTest" =! false

    Environment.SetEnvironmentVariable("envExistsTest", "test")
    envExists "envExistsTest" =! true
