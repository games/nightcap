module Nightcap.Codecs.Json

open System.IO
open System.Text.Json
open System.Text.Json.Serialization


let configure (tagName: string) (fieldsName: string) (options: JsonSerializerOptions) =
    JsonFSharpConverter(
        unionTagName = tagName,
        unionFieldsName = fieldsName,
        unionEncoding =
            (JsonUnionEncoding.UnwrapOption
             ||| JsonUnionEncoding.UnwrapFieldlessTags
             ||| JsonUnionEncoding.UnwrapSingleCaseUnions
             ||| JsonUnionEncoding.UnwrapSingleFieldCases)
    )
    |> options.Converters.Add

    options.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
    options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    options

let defaultOptions =
    JsonSerializerOptions(JsonSerializerDefaults.Web) |> configure "kind" "value"

let decode<'T> (json: string) =
    JsonSerializer.Deserialize<'T>(json, defaultOptions)

let tryDecode<'T> (json: string) =
    try
        JsonSerializer.Deserialize<'T>(json, defaultOptions) |> Some
    with :? JsonException ->
        None

let decodeAsync<'T> (stream: Stream) =
    JsonSerializer.DeserializeAsync<'T>(stream, defaultOptions)

let encode<'T> (data: 'T) =
    JsonSerializer.Serialize(data, defaultOptions)

let parse (json: string) = JsonDocument.Parse(json)
