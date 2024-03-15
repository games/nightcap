module Trio.Web.Tests.HttpContext

open Xunit
open System.Net
open Microsoft.AspNetCore.Http
open Trio.Web.HttpContext


let httpContext (headers: (string * string) list) (ipAddress: string) =
    let ctx = DefaultHttpContext()
    headers |> Seq.iter ctx.Request.Headers.Add
    ctx.Connection.RemoteIpAddress <- IPAddress.Parse ipAddress
    ctx


[<Theory>]
[<InlineData("192.168.35.4, 192.168.10.10, 172.0.01", "192.168.35.4")>]
[<InlineData("127.0.0.1, 192.168.10.10, 172.0.01", "127.0.0.1")>]
[<InlineData("127.0.0.1,192.168.10.10,172.0.01", "127.0.0.1")>]
[<InlineData("123, 200.100.100.100", "0.0.0.123")>]
[<InlineData("abc, def, 127.0.0.1", "127.0.0.1")>]
let ``Parse IP from IP list string should pick the first IP`` (addresses: string) (expected: string) =
    let first = firstIpAddress addresses
    Assert.Equal(Some(IPAddress.Parse expected), first)

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData("abc")>]
[<InlineData("abc, aaa, ddd")>]
let ``Parse IP from IP list string should return None if there is not valid IP`` (addresses: string) =
    let result = firstIpAddress addresses
    Assert.Equal(None, result)

[<Theory>]
[<InlineData("192.168.35.4, 192.168.10.10, 172.0.01", "CF-Connecting-IP", "222.127.130.20", "192.168.35.4")>]
[<InlineData("192.168.35.4, 192.168.10.10, 172.0.01", "X-Real-IP", "222.127.130.20", "192.168.35.4")>]
[<InlineData("192.168.35.4, 192.168.10.10, 172.0.01", "X-Forwarded-For", "222.127.130.20", "192.168.35.4")>]
[<InlineData("", "", "222.127.130.20", "222.127.130.20")>]
let ``Should be able to extract from HTTP Context``
    (addresses: string)
    (headerName: string)
    (remoteIp: string)
    (expected: string)
    =
    let ctx = httpContext [ headerName, addresses ] remoteIp
    let ip = remoteIpAddress ctx

    Assert.Equal((IPAddress.Parse expected), ip)
