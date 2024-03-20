module Nightcap.Tests.ShortGuid

open System
open Xunit
open Swensen.Unquote
open Nightcap.ShortGuid

[<Fact>]
let ``short guid should works`` () =
    let guid = Guid.NewGuid()
    let sid = encode guid
    let decoded = decode sid

    Seq.length sid =! 22
    guid =! decoded

[<Fact>]
let ``short guid should works with custom alphabet`` () =
    let short = create ()
    let guid = decode short
    Assert.NotNull guid
    