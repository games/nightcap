module Nightcap.Result.Operators

open FSharpPlus

let (?>) (m: Result<'T, 'E>) (v: 'D) = m >>= (fun _ -> Ok v)

let ( *> ) (m1: Result<'T, 'E>) (m2: Result<'T, 'E>) = m1 >>= (fun _ -> m2)
