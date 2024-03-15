module Trio.Web.HttpContext

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Net
open System.Net.Sockets
open Microsoft.AspNetCore.Http
open FSharpPlus



let readBodyAsString (ctx: HttpContext) =
    task {
        let stream = ctx.Request.Body
        use reader = new StreamReader(stream)
        let body = reader.ReadToEndAsync()
        return! body
    }

let firstOrDefault<'T when 'T: null> (it: IEnumerable<'T>) = it.FirstOrDefault() |> Option.ofObj

let tryParseIpAddress (address: string) =
    match IPAddress.TryParse address with
    | true, ip when
        ip.AddressFamily = AddressFamily.InterNetwork
        || ip.AddressFamily = AddressFamily.InterNetworkV6
        ->
        Some ip
    | _ -> None

let firstIpAddress (addresses: string) =
    if String.IsNullOrWhiteSpace addresses then
        None
    else
        let ips =
            addresses.Split(",", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

        ips |> Seq.tryPick tryParseIpAddress

let remoteIpAddress (ctx: HttpContext) =
    let headers = ctx.Request.Headers

    firstOrDefault headers["CF-Connecting-IP"]
    <|> firstOrDefault headers["X-Real-IP"]
    <|> firstOrDefault headers["X-Forwarded-For"]
    >>= firstIpAddress
    |> Option.defaultValue ctx.Connection.RemoteIpAddress
