module Nightcap.Seq

open System.Collections.Generic
open System.Linq


let firstOrDefault<'T when 'T: null> (it: IEnumerable<'T>) = it.FirstOrDefault() |> Option.ofObj
