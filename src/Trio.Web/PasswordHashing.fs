module Trio.Web.PasswordHashing


// https://github.com/dotnet/aspnetcore/blob/4963b764e3c03473022c75f13b7f82c531650001/src/Identity/Extensions.Core/src/PasswordHasher.cs

open System
open System.Security.Cryptography
open Microsoft.AspNetCore.Cryptography.KeyDerivation
open FSharp.UMX

[<Measure>]
type password

[<Measure>]
type hashedPassword


let writeNetworkByteOrder (buffer: byte[]) (offset: int) (value: uint) =
    buffer[offset + 0] <- byte (value >>> 24)
    buffer[offset + 1] <- byte (value >>> 16)
    buffer[offset + 2] <- byte (value >>> 8)
    buffer[offset + 3] <- byte (value >>> 0)

let readNetworkByteOrder (buffer: byte[]) (offset: int) =
    (uint (buffer[offset + 0]) <<< 24)
    ||| (uint (buffer[offset + 1]) <<< 16)
    ||| (uint (buffer[offset + 2]) <<< 8)
    ||| (uint (buffer[offset + 3]))

let hash (password: string<password>) : string<hashedPassword> =
    let saltSize = 128 / 8 // 128 bits
    let salt = RandomNumberGenerator.GetBytes saltSize
    let prf = KeyDerivationPrf.HMACSHA512
    let iteration = 100_000
    let numBytesRequested = 256 / 8

    let subKey =
        KeyDerivation.Pbkdf2(%password, salt, prf, iteration, numBytesRequested)

    let outputSize = 13 + salt.Length + subKey.Length
    let outputBytes = Array.zeroCreate<byte> outputSize
    outputBytes[0] <- 01uy
    writeNetworkByteOrder outputBytes 1 (uint prf)
    writeNetworkByteOrder outputBytes 5 (uint iteration)
    writeNetworkByteOrder outputBytes 9 (uint saltSize)
    Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length)
    Buffer.BlockCopy(subKey, 0, outputBytes, 13 + saltSize, subKey.Length)
    let hashed = Convert.ToBase64String outputBytes
    %hashed

let verifyHashedPassword (hashedPassword: string<hashedPassword>) (providedPassword: string<password>) =
    let decodedHashed = Convert.FromBase64String %hashedPassword

    if decodedHashed.Length = 0 then
        false
    elif decodedHashed[0] <> 01uy then
        false
    else
        let prf = readNetworkByteOrder decodedHashed 1 |> int |> enum<KeyDerivationPrf>

        let iteration = readNetworkByteOrder decodedHashed 5 |> int

        let saltLength = readNetworkByteOrder decodedHashed 9 |> int

        if saltLength < 128 / 8 then
            false
        else
            let salt = Array.zeroCreate<byte> saltLength
            Buffer.BlockCopy(decodedHashed, 13, salt, 0, salt.Length)
            let subKeyLength = decodedHashed.Length - 13 - salt.Length

            if subKeyLength < 128 / 8 then
                false
            else
                let expectedSubKey = Array.zeroCreate subKeyLength
                Buffer.BlockCopy(decodedHashed, 13 + salt.Length, expectedSubKey, 0, expectedSubKey.Length)

                let actualSubKey =
                    KeyDerivation.Pbkdf2(%providedPassword, salt, prf, iteration, subKeyLength)

                CryptographicOperations.FixedTimeEquals(ReadOnlySpan actualSubKey, ReadOnlySpan expectedSubKey)
