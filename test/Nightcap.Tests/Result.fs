module Nightcap.Tests.Result

open Xunit
open Swensen.Unquote
open Nightcap.Validation
open Nightcap.Result.Operators


[<Fact>]
let `` (?>) should return the next value if the first parameter is Ok`` () =
    let m1 = Ok "whatever"
    let x = m1 ?> "result"
    x =! Ok "result"

[<Fact>]
let `` (?>) should return the error if first parameter is Error`` () =
    let m1 = Error "error"
    let x = m1 ?> "result"
    x =! Error "error"

[<Fact>]
let `` (*>) bind two Result<'T> and return the second one if the first is Ok`` () =
    let m1 = Ok 1
    let m2 = Ok 2
    let x = m1 *> m2
    x =! Ok 2

[<Fact>]
let `` (*>) bind two Result<'T> and return the first one if the first is Error`` () =
    let m1 = Error "error"
    let m2 = Ok 2
    let x = m1 *> m2
    x =! Error "error"


let toResult (condition: 'T -> bool) errorMessage (x: 'T) =
    if condition x then Ok x else Error errorMessage

[<Fact>]
let `` composition should works`` () =

    let mustBePresent = toResult isNotNullOrWhiteSpace "String is null or empty"
    let mustUrlSafely = toResult isUrlSafely "String is not URL safely"
    let mustBetAlphanumeric = toResult isAlphanumeric "String is not alphanumeric"

    let a =
        mustBePresent "abc" *> mustUrlSafely "a-b-c" *> mustBetAlphanumeric "abc123"
        ?> "All Good"

    a =! Ok "All Good"

    let b =
        mustBePresent "abc" *> mustUrlSafely "!@#$%" *> mustBetAlphanumeric "abc123"
        ?> "All Good"

    b =! Error "String is not URL safely"

    let c =
        mustBePresent "abc" *> mustUrlSafely "a-b-c" *> mustBetAlphanumeric "abc123@"
        ?> "All Good"

    c =! Error "String is not alphanumeric"
