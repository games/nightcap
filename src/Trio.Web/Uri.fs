module Trio.Web.Uri


open System
open System.Net
open Microsoft.AspNetCore.Http


type QueryStringName = string

type QueryStringValue = string

type QueryStringCollection = Map<QueryStringName, List<QueryStringValue>>


let merge m1 m2 =
    Map.fold (fun s k v -> Map.add k v s) m1 m2

let private uriEncode: _ -> string =
    List.map (fun kv -> String.Concat [ WebUtility.UrlEncode(fst kv); "="; WebUtility.UrlEncode(snd kv) ])
    >> String.concat "&"

let stringify (queryStringItems: QueryStringCollection) =
    if Map.isEmpty queryStringItems then
        ""
    else
        let items =
            Map.toList queryStringItems
            |> List.collect (fun (k, vs) -> vs |> List.map (fun v -> k, v))

        String.Concat [ uriEncode items ]

let buildUri (url: string) (queryStringItems: QueryStringCollection) =
    UriBuilder(Uri url, Query = stringify queryStringItems).Uri

let queryString (args: seq<QueryStringName * QueryStringValue>) =
    args |> Seq.map System.Collections.Generic.KeyValuePair |> QueryString.Create

let withQuery (uri: Uri) (args: seq<QueryStringName * QueryStringValue>) =
    UriBuilder(uri, Query = (queryString args).Value).Uri
