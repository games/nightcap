module Trio.RandomNumber

open System
open System.Security.Cryptography


let private reciprocal = 1.0 / 4294967296.0

let float () =
    RandomNumberGenerator.GetBytes 4
    |> BitConverter.ToUInt32
    |> float
    |> (*) reciprocal

let sample (choices: 'T array) =
    requires (choices.Length > 0) "Choices can't be empty"
    let i = RandomNumberGenerator.GetInt32 choices.Length
    choices[i]


type IRandomNumber =
    abstract member Int32: unit -> int
    abstract member Int32: exclusive: int -> int
    abstract member Float: unit -> float

let shared =
    { new IRandomNumber with

        member this.Int32() =
            RandomNumberGenerator.GetInt32 Int32.MaxValue

        member this.Int32 exclusive =
            RandomNumberGenerator.GetInt32 exclusive

        member this.Float() = float () }
