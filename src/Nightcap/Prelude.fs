namespace Nightcap

open System

[<AutoOpen>]
module Prelude =
    let inline notNull x = (isNull >> not) x

    let inline requires condition message =
        if not condition then
            failwith message

    let orJust = Option.defaultValue

    let orWith = Option.defaultWith

    let envVars name =
        Environment.GetEnvironmentVariable name |> Option.ofObj

    let requiredEnv name =
        let var = envVars name

        if var.IsNone then
            failwith $"Environment variable not found: {name}"

        var.Value

    let envExists name = (envVars name).IsSome

    module Operators =

        let (|?) a b = defaultArg b a

        let inline (!!) (x: ^a) : ^b =
            ((^a or ^b): (static member op_Implicit: ^a -> ^b) x)
