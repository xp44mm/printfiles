namespace Markdown

open Xunit
open Xunit.Abstractions

open System.IO
open System.Text.RegularExpressions
open MarkdownLineParser

type MarkdownLineTest(output: ITestOutputHelper) =
    let text =
        Path.Combine(@"C:\Program Files\Typora\resources\app\Docs","Markdown Reference.md")
        |> File.ReadAllText

    let lines = Regex.Split(text,@"\r?\n")

    [<Fact>]
    member this.``Header Test``() =

        let blocks = ParseDoc [] <| List.ofArray lines
        File.WriteAllText(
            @"d:\markdown blocks.txt",
            [
                for b in blocks do
                    yield Render.normalize b
                    //match b with
                    //| MarkdownLine.TOC _
                    //| MarkdownLine.Footnote _
                    //| MarkdownLine.LinkReferenceDefinition _ ->
                    //    yield sprintf "%A" b
                    //| _ -> ()
        
            ]|>String.concat "")//¶\n

    [<Fact>]
    member this.``TableAlign Test``() =
        let text =
            Path.Combine(__SOURCE_DIRECTORY__,"pipeTableExample.md")
            |> File.ReadAllText

        let lines = Regex.Split(text, @"\r?\n")

        for line in lines do
            let s =
                match line with
                | TableDelimiter cols ->
                    sprintf "%A" cols
                | _ -> line
            output.WriteLine(s)

        
