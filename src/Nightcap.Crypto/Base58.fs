module Nightcap.Crypto.Base58


open System
open System.Security.Cryptography

// This is taken from:
// http://www.fssnip.net/m6/title/Randomkeygenerator-and-Base58encoding
let private codeString =
    [ '1' .. '9' ]
    @ [ 'A' .. 'H' ]
    @ [ 'J' .. 'N' ]
    @ [ 'P' .. 'Z' ]
    @ [ 'a' .. 'k' ]
    @ [ 'm' .. 'z' ]
    |> List.toArray

let encode (hash: byte[]) =

    let data = hash |> Array.toList

    let rec toBigInt =
        function
        | [], acc -> acc
        | h :: t, acc -> toBigInt (t, acc * 256I + bigint (int h))

    let rec base58encodeLeft =
        function
        | i, acc when i > 0I ->
            let reminder = ref 0I
            let dividend = bigint.DivRem(i, 58I, reminder)
            let char = codeString[int reminder.contents]
            base58encodeLeft (dividend, char :: acc)
        | _, acc -> acc

    let appendOnes =
        let rec insertOnes =
            function
            | h :: t, acc when h = 0uy -> insertOnes (t, '1' :: acc)
            | _, acc -> acc

        insertOnes (data, [])

    let big = toBigInt (data, 0I)
    let encoded = appendOnes @ base58encodeLeft (big, []) |> List.toArray
    String encoded

let encodeCheck (input: byte[]) =
    let checksum = (input |> SHA256.HashData |> SHA256.HashData)[..3]
    let addressCheck = Array.append input checksum
    encode addressCheck
