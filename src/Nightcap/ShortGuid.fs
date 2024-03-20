module Nightcap.ShortGuid

open System
open FSharp.UMX

[<Measure>]
type shortGuid


let encode (guid: Guid) : string<shortGuid> =
    %Convert
        .ToBase64String(guid.ToByteArray())
        .Replace("/", "_")
        .Replace("+", "-")
        .Substring(0, 22)


let decode (shortGuid: string<shortGuid>) =
    let base64 = (%shortGuid: string).Replace("_", "/").Replace("-", "+")
    let bytes = Convert.FromBase64String($"{base64}==")
    Guid(bytes)


let create () = encode (Guid.NewGuid())
