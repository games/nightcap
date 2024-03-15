namespace Nightcap

[<AutoOpen>]
module Prelude =
    let inline notNull x = (isNull >> not) x

    let inline requires condition message =
        if not condition then
            failwith message

    let orJust = Option.defaultValue