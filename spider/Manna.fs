namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO

open AngleSharp
open AngleSharp.Dom

open FSharp.Control.Tasks.V2

/// 少女之心(曼娜回忆录)
type MannaTest(output: ITestOutputHelper) =

    //[<Fact>]
    member this. ``norm source`` () =
        let htmlFileDir = @"C:\Users\cuishengli\source\repos\xp44mm\MemoriesOfGirl"
        let file = Path.Combine(htmlFileDir,"source.html")

        let removeBr (e:IElement) =
            e.ChildNodes
            |> Seq.map(fun node-> node.Clone())
            |> Seq.filter(fun node -> node.NodeType = NodeType.Text)
            |> Seq.map(fun node ->
                let p = e.Owner.CreateElement("P")
                p.AppendChild node |> ignore
                p
            )
        
        
        task {
            let! text = File.ReadAllTextAsync(file)
            let context = BrowsingContext.New(Configuration.Default)

            let! document = 
                context.OpenAsync(fun req -> req.Content(text) |> ignore)


            let content =
                [
                    yield document.Body.FirstElementChild
                    yield! removeBr document.Body.LastElementChild
                ]
                |> List.filter(fun e -> e.HasChildNodes)
                |> List.map(fun e -> 
                    e.FirstChild.NodeValue <- e.FirstChild.NodeValue.Trim()
                    e
                    )
                |> List.filter(fun e -> e.FirstChild.NodeValue <> "")
                |> List.map(fun e -> e.OuterHtml)
                |> String.concat "\n"

            let targetFile = Path.Combine(htmlFileDir,"target.html")
            do! File.WriteAllTextAsync(targetFile,content)
        }
    [<Fact>]
    member this. ``norm html to text`` () =
        let htmlFileDir = @"C:\Users\cuishengli\source\repos\xp44mm\MemoriesOfGirl"
        let file = Path.Combine(htmlFileDir,"norm.html")
        
        task {
            let! text = File.ReadAllTextAsync(file)
            let context = BrowsingContext.New(Configuration.Default)

            let! document = 
                context.OpenAsync(fun req -> req.Content(text) |> ignore)

            let content =
                document.Body.Children
                |> Seq.map(fun e -> e.TextContent)
                |> String.concat "\n"

            let targetFile = Path.Combine(htmlFileDir,"MemoriesOfGirl.txt")
            do! File.WriteAllTextAsync(targetFile,content)
        }
