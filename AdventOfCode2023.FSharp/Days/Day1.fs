module Day1
    open System.IO

    let firstInt line =
        line
        |> Seq.map string
        |> Seq.map System.Int32.TryParse
        |> Seq.filter (fun (a,_) -> a)
        |> Seq.head
        |> snd

    let parseLine s = firstInt(s) * 10 + firstInt(Seq.rev(s))        

    let solvePart1() =
        "Input/day1.txt"
        |> File.ReadLines
        |> Seq.map parseLine
        |> Seq.sum