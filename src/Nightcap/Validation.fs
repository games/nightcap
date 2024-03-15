module Nightcap.Validation


open System
open System.Linq
open System.ComponentModel.DataAnnotations
open FSharpPlus


let anyNullOrWhiteSpaces (items: string seq) =
    Seq.isEmpty items || Seq.exists String.IsNullOrWhiteSpace items

let isNotNullOrWhiteSpace = String.IsNullOrWhiteSpace >> not

let isValidEmailAddress (address: string) =
    isNotNullOrWhiteSpace address && EmailAddressAttribute().IsValid(address)

let isLesserThan (length: int) (str: string) =
    isNotNullOrWhiteSpace str && str.Trim().Length < length

let isUpper (c: char) = Char.IsUpper c

let IsLower (c: char) = Char.IsLower c

let isDigit (c: char) = Char.IsDigit c

let isLetterOrDigit (c: char) = Char.IsLetterOrDigit c

let isAlphanumeric (str: string) =
    isNotNullOrWhiteSpace str && str.All isLetterOrDigit

let isUrlSafely (str: string) =
    isNotNullOrWhiteSpace str
    && str.All(fun c -> isLetterOrDigit c || c = '-' || c = '_')

let isStrongPassword (password: string) =
    isNotNullOrWhiteSpace password
    && (password.Length >= 12)
    && not (password.All isLetterOrDigit)
    && password.Any(isDigit)
    && password.Any(isUpper)
    && password.Any(IsLower)
    && password.Distinct().Count() >= 5

let isNotStrongPassword = isStrongPassword >> not

let isNoneOrPositive (value: decimal option) = map ((<) 0m) value |> orJust true

let isMaxLength (length: int) (str: string) = notNull str && str.Length <= length

let maybeMaxLength (length: int) (str: string option) =
    map (isMaxLength length) str |> orJust true
