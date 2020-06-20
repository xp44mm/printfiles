namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO

open AngleSharp
open AngleSharp.Dom

open FSharp.Control.Tasks.V2
open System.Threading.Tasks
open AngleSharp.Browser
open System.Reactive.Linq

/// 少女之心(曼娜回忆录)
type MannaTest(output: ITestOutputHelper) =
    let root = @"C:\Users\cuishengli\source\repos\xp44mm\MemoriesOfGirl"

    [<Fact>]
    member this. ``norm123456`` () =
        let removeBr (e:IElement) =
            let res = ResizeArray<ResizeArray<INode>>()
            res.Add(ResizeArray<INode>())

            e.ChildNodes
            |> Seq.iter(fun node ->
                if node.NodeType = NodeType.Element then
                    let elem = node :?> IElement
                    if elem.NodeName = "BR" then
                        res.Add(ResizeArray<INode>())
                    elif elem.NodeName = "FONT" then
                        ()
                    else
                        res.[res.Count-1].Add(elem)
                else
                    res.[res.Count-1].Add node
            )
        
            res
            |> Array.ofSeq
            |> Array.filter(fun childNodes -> childNodes.Count > 0)
            |> Array.map(fun childNodes ->
                let wp = e.Owner.CreateElement("P")
                childNodes
                |> Seq.iter(fun node ->
                    wp.AppendChild node
                    |> ignore
                )
                wp
            )
        
        let source = Path.Combine(root,"star")
        let targetDir = Path.Combine(root, "target")

        //删除目录下的所有文件
        Directory.GetFiles(targetDir)
        |> Array.iter(fun f -> File.Delete(f))

        let tcs = TaskCompletionSource<string>()

        let angleSharpConfig = Configuration.Default.Without<EncodingMetaHandler>()

        Directory.GetFiles(source)
            .ToObservable()
            .Select(fun file ->
                task {
                    let! text = File.ReadAllTextAsync(file)
                    let context = BrowsingContext.New(angleSharpConfig)

                    let! document = 
                        context.OpenAsync(fun req -> req.Content(text) |> ignore)

                    ///
                    let container = 
                        document.QuerySelector("body > table > tbody > tr > td > table:nth-child(5) > tbody > tr > td")

                    let content =
                        container.Children
                        |> Seq.filter(fun e -> e.TagName <> "HR")
                        |> Seq.toArray

                    let content = 
                        content.[0..content.Length-2]
                        |> Array.collect(removeBr)
                        |> Array.filter(fun e -> e.TextContent.Trim() <> "")
                        |> Array.map(fun e -> 
                            e.TextContent <- e.TextContent.Trim()
                            e.OuterHtml)
                        |> String.concat "\n"

                    let targetFileName =
                        Path.GetFileName(file)
                        |> fun file -> Path.Combine(targetDir,file)
                    do! File.WriteAllTextAsync(targetFileName,content)
                }
            )
            .Merge()
            //.Synchronize()
            //.ObserveOn(SynchronizationContext.Current)
            .Subscribe(
                (fun () -> ()),
                (fun () ->
                    output.WriteLine("done!")
                    tcs.SetResult("done!"))
            )
        |> ignore

        tcs.Task

    //[<Fact>]
    member this. ``norm source`` () =
        let file = Path.Combine(root,"source.html")

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

            let targetFile = Path.Combine(root,"target.html")
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
