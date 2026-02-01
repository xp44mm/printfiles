namespace spider

open Xunit
open Xunit.Abstractions

open System.IO
open System.Text
open System.Text.RegularExpressions
open FSharp.Idioms

type NewConceptEnglishBookTest(output: ITestOutputHelper) =

    [<Fact>]
    member this.``修改标题`` () =
        let t = @"C:\Users\cuishengli\Documents\崔安琪\新概念英语第一册目录.txt"
        let n = @"C:\Users\cuishengli\Documents\崔安琪\NCE1.txt"

        let tlines = File.ReadAllLines(t,UTF8Encoding.UTF8)
        let nlines = File.ReadAllLines(n,UTF8Encoding.UTF8)

        output.WriteLine($"{tlines}")
