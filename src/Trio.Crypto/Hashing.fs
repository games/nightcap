module Trio.Crypto.Hashing

open System
open System.Security.Cryptography
open System.Text
open Org.BouncyCastle.Crypto.Digests

let keccak input =
    let digest = KeccakDigest 256
    let output = Array.zeroCreate (digest.GetDigestSize())
    digest.BlockUpdate(input, 0, input.Length)
    digest.DoFinal(output, 0) |> ignore
    output


let sha256 (input: string) =
    input |> Encoding.UTF8.GetBytes |> SHA256.HashData |> Convert.ToBase64String
