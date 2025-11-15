module Nightcap.Web.HttpContext

open System
open System.Buffers
open System.IO
open System.IO.Pipelines
open System.Net
open System.Net.Sockets
open Microsoft.AspNetCore.Http
open FSharpPlus


let readBodyAsString (ctx: HttpContext) =
    task {
        let stream = ctx.Request.Body
        use reader = new StreamReader(stream)
        return! reader.ReadToEndAsync()
    }


let readBytesFromPipeReader (reader: PipeReader) =
    task {
        let mutable buffer = Unchecked.defaultof<ReadOnlySequence<byte>>
        let mutable isCompleted = false

        while not isCompleted do
            let! result = reader.ReadAsync()
            buffer <- result.Buffer
            isCompleted <- result.IsCompleted || result.IsCanceled

            if not isCompleted then
                reader.AdvanceTo(result.Buffer.Start, result.Buffer.End)

        return buffer.ToArray()
    }


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

    Seq.tryHead headers["CF-Connecting-IP"]
    <|> Seq.tryHead headers["X-Real-IP"]
    <|> Seq.tryHead headers["X-Forwarded-For"]
    >>= firstIpAddress
    |> Option.defaultValue ctx.Connection.RemoteIpAddress
