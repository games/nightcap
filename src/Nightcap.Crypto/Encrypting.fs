module Nightcap.Crypto.Encrypting

open System.IO
open System.Security.Cryptography


module AES =

    let private performCryptography (transform: ICryptoTransform) (data: byte[]) =
        use ms = new MemoryStream()
        use cs = new CryptoStream(ms, transform, CryptoStreamMode.Write)
        do cs.Write(data, 0, data.Length)
        do cs.FlushFinalBlock()
        ms.ToArray()

    let private performCryptographyAsync (transform: ICryptoTransform) (data: byte[]) =
        task {
            use ms = new MemoryStream()
            use cs = new CryptoStream(ms, transform, CryptoStreamMode.Write)
            do! cs.WriteAsync(data, 0, data.Length)
            do! cs.FlushFinalBlockAsync()
            return ms.ToArray()
        }

    let encrypt (key: byte[]) (iv: byte[]) (data: byte[]) =
        use aes =
            Aes.Create(Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7)

        let encryptor = aes.CreateEncryptor()
        performCryptography encryptor data

    let decrypt (key: byte[]) (iv: byte[]) (data: byte[]) =
        use aes =
            Aes.Create(Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7)

        let decryptor = aes.CreateDecryptor()
        performCryptography decryptor data

    let encryptAsync (key: byte[]) (iv: byte[]) (data: byte[]) =
        task {
            use aes =
                Aes.Create(Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7)

            let encryptor = aes.CreateEncryptor()
            return! performCryptographyAsync encryptor data
        }

    let decryptAsync (key: byte[]) (iv: byte[]) (data: byte[]) =
        task {
            use aes =
                Aes.Create(Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7)

            let decryptor = aes.CreateDecryptor()
            return! performCryptographyAsync decryptor data
        }
