module Trio.Web.Tests.Uri


open System
open Xunit
open Trio.Web


[<Fact>]
let ``build URL from seq args should works`` () =
    let qs = Uri.queryString [ "a", "b"; "a", "c"; "b", "!@#/\$%^&" ]
    Assert.Equal("?a=b&a=c&b=!@%23%2F%5C$%25%5E%26", qs.Value)

[<Fact>]
let ``build URL with query should works`` () =
    let qs =
        Uri.withQuery (Uri "https://www.google.com") [ "a", "b"; "a", "c"; "b", "!@#/\$%^&" ]

    Assert.Equal(Uri "https://www.google.com?a=b&a=c&b=!@%23%2F%5C$%25%5E%26", qs)

[<Fact>]
let ``build URL with query should works 2`` () =
    let baseUri = Uri "https://www.google.com"

    let qs =
        Uri.withQuery (Uri(baseUri, "/path")) [ "a", "b"; "a", "c"; "b", "!@#/\$%^&" ]

    Assert.Equal(Uri "https://www.google.com/path?a=b&a=c&b=!@%23%2F%5C$%25%5E%26", qs)
