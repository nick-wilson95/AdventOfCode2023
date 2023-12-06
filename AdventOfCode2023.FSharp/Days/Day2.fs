module Day2
    open System.Collections.Generic
    open System.IO
    open Utility

    type Game = { Id: int; Rounds : IDictionary<string, int>[] } with
        member this.MaxBallsByColour colour =
            this.Rounds |> Seq.map (valueOrDefault colour 0) |> Seq.max

    let parseRound roundText =
        roundText
        |> split ','
        |> Seq.map (fun x -> x.Trim().Split(' '))
        |> Seq.map (fun x -> (x[1], int(x[0])))
        |> dict

    let parseLine line =
        line
        |> split ':'
        |> Seq.item 1
        |> split ';'
        |> Seq.map parseRound
        |> Seq.toArray

    let parseInput() =
        "Input/day2.txt"
        |> File.ReadLines
        |> Seq.map parseLine
        |> Seq.indexed
        |> Seq.map (fun x -> {Id = fst(x) + 1; Rounds = snd(x)})

    let validateGame (game : Game) =
        game.MaxBallsByColour "red" < 13
        && game.MaxBallsByColour "green" < 14
        && game.MaxBallsByColour "blue" < 15

    let getPower (game : Game) =
        game.MaxBallsByColour "red"
        * game.MaxBallsByColour "green"
        * game.MaxBallsByColour "blue"

    let solvePart1() =
        parseInput()
        |> Seq.filter validateGame
        |> Seq.map (fun x -> x.Id)
        |> Seq.sum

    let solvePart2() =
        parseInput()
        |> Seq.map getPower
        |> Seq.sum