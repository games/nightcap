namespace Nightcap

[<AutoOpen>]
module Prelude =
    let inline notNull x = (isNull >> not) x

    let inline requires condition message =
        if not condition then
            failwith message

    let orJust = Option.defaultValue
    
    let orWith = Option.defaultWith

    module Operators =

        let (|?) a b = defaultArg b a

        let inline (!!) (x: ^a) : ^b =
            ((^a or ^b): (static member op_Implicit: ^a -> ^b) x)
