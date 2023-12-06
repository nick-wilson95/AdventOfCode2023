module Utility
    open System.Collections.Generic

    let split (c : char) (str : string) = str.Split(c)

    let valueOrDefault<'a,'b> (key : 'a) (defaultValue: 'b) (dict : IDictionary<'a,'b>) =
        match dict.ContainsKey(key) with
            | true -> dict[key]
            | false -> defaultValue

