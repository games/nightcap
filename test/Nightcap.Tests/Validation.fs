module Nightcap.Tests

open Xunit
open Swensen.Unquote
open Nightcap.Validation


[<Theory>]
[<InlineData(null, false)>]
[<InlineData("", false)>]
[<InlineData(" ", false)>]
[<InlineData("                                     ", false)>]
[<InlineData("abc123", false)>]
[<InlineData("Abc123", false)>]
[<InlineData("Abc12356789", false)>]
[<InlineData("Abc1234&^*", false)>]
[<InlineData("AaAaaaaaaaaaaaaaa1111111111111111111*", false)>]
[<InlineData("Aaaa1111****", false)>]
[<InlineData("2016-08-12 17:03:01.944", false)>]
[<InlineData("jpaidfy071208301zn()&%%*)^%12", false)>]
[<InlineData("jpAidfy071208301zn()&%%*)^%12", true)>]
let ``Valid strong password should works`` (password: string, isStrong: bool) = isStrong =! isStrongPassword password


[<Theory>]
[<InlineData("abc", true)>]
[<InlineData("Abc", true)>]
[<InlineData("ABC", true)>]
[<InlineData("abcB", true)>]
[<InlineData("abcBB123", true)>]
[<InlineData("123", true)>]
[<InlineData("as123", true)>]
[<InlineData("AS123", true)>]
[<InlineData("AS 123", false)>]
[<InlineData("as 123", false)>]
[<InlineData(" as123 ", false)>]
[<InlineData(null, false)>]
[<InlineData("   ", false)>]
[<InlineData("a_b", false)>]
[<InlineData("a/b", false)>]
[<InlineData("a-b", false)>]
let ``Valid is alphanumeric should works`` (str: string, isTrue: bool) = isTrue =! isAlphanumeric str


[<Theory>]
[<InlineData("", false)>]
[<InlineData("   ", false)>]
[<InlineData(null, false)>]
[<InlineData("abc~1", false)>]
[<InlineData("abc!1", false)>]
[<InlineData("abc@1", false)>]
[<InlineData("abc#$%^&*()_+1", false)>]
[<InlineData("abc/1", false)>]
[<InlineData("abc 1", false)>]
[<InlineData("ab c 1", false)>]
[<InlineData("ab\c\1", false)>]
[<InlineData("abc_1", true)>]
[<InlineData("abc-1", true)>]
[<InlineData("Abc-1", true)>]
[<InlineData("ABC-1", true)>]
let ``Valid if string is URL safely should works`` (str: string, isTrue: bool) = isTrue =! isUrlSafely str


[<Fact>]
let ``Valid if amount is none or positive should works`` () =
    let test (amount: decimal option) (isTrue: bool) = isTrue =! isNoneOrPositive amount

    test None true
    test (Some 0.0001m) true
    test (Some 1m) true
    test (Some 10000000000m) true
    test (Some 0m) false
    test (Some -0.01m) false
    test (Some -0.00001m) false
    test (Some -100000m) false


[<Theory>]
[<InlineData("", 0, true)>]
[<InlineData("abc", 0, false)>]
[<InlineData("abc", 1, false)>]
[<InlineData("abc", 2, false)>]
[<InlineData("abc", 3, true)>]
[<InlineData("abc", 4, true)>]
[<InlineData(null, 0, false)>]
[<InlineData(null, 4, false)>]
let ``Valid if is max length should works`` (str: string, max: int, isTrue: bool) = isTrue =! isMaxLength max str

[<Fact>]
let ``Valid if maybe is max length should works`` () =
    let test (str: string option) max (isTrue: bool) = isTrue =! maybeMaxLength max str

    test None 0 true
    test (Some null) 0 false
    test (Some "") 0 true
    test (Some "") 1 true
    test (Some "abc") 1 false
    test (Some "abc") 3 true
    test (Some "abc") 4 true


[<Fact>]
let ``anyNullOrWhiteSpaces should works`` () =
    anyNullOrWhiteSpaces [] =! true
    anyNullOrWhiteSpaces [ null ] =! true
    anyNullOrWhiteSpaces [ "" ] =! true
    anyNullOrWhiteSpaces [ " " ] =! true
    anyNullOrWhiteSpaces [ ""; "" ] =! true
    anyNullOrWhiteSpaces [ " "; " " ] =! true
    anyNullOrWhiteSpaces [ " "; "" ] =! true
    anyNullOrWhiteSpaces [ " "; null ] =! true
    anyNullOrWhiteSpaces [ "abc"; "" ] =! true
    anyNullOrWhiteSpaces [ "abc"; " " ] =! true
    anyNullOrWhiteSpaces [ "abc"; null ] =! true
    anyNullOrWhiteSpaces [ "abc"; " dec" ] =! false

[<Theory>]
[<InlineData("abc@google.com", true)>]
[<InlineData("abc.dec@google.com", true)>]
[<InlineData(null, false)>]
[<InlineData("", false)>]
[<InlineData("  ", false)>]
[<InlineData("abcd345", false)>]
[<InlineData("abc@asdf", true)>]
[<InlineData(" abc@asdf  ", true)>]
[<InlineData("abc@asdf.dd", true)>]
let ``isValidEmailAddress should works`` (email: string) (isTrue: bool) = isTrue =! isValidEmailAddress email

[<Theory>]
[<InlineData("", -1, false)>]
[<InlineData("a", 0, false)>]
[<InlineData("ab", 1, false)>]
[<InlineData("ab", 2, false)>]
[<InlineData(null, -100, false)>]
[<InlineData("     ", 10, false)>]
[<InlineData("abc", 10, true)>]
[<InlineData("  abc   ", 4, true)>]
let ``isLesserThan should works`` (str: string) (length: int) (isTrue: bool) = isLesserThan length str =! isTrue
