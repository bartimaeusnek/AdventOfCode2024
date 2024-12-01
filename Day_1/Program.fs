// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

open System
open System.IO

//Loading Code for the input file
let getFileContent fileName =
    use sr = File.OpenText(fileName)
    sr.ReadToEnd()

let buildLists (fileContent: string) =
    fileContent.Split("\n", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
        |> Seq.map _.Split(" ", StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
        |> Seq.map (fun arr -> arr[0] |> Int32.Parse, arr[1] |> Int32.Parse)
        |> Seq.fold (fun (accA, accB) (a, b) -> a::accA, b::accB) ([], [])

//Solution for Part 1
let calculateTotalDistance arrA arrB =
    Seq.mapi2 (fun _ a b -> if (a > b) then a - b else b - a) (arrA |> Seq.sort) (arrB |> Seq.sort)
        |> Seq.sum

//Solution for Part 2
let calculateTimes arrA arrB =
    Seq.map(fun a -> a * (Seq.filter (fun b -> b = a) arrB |> Seq.length)) arrA
        |> Seq.sum

let listA, listB = buildLists (getFileContent "input")
calculateTotalDistance listA listB |> Console.WriteLine
calculateTimes listA listB |> Console.WriteLine