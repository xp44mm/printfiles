namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Text
open System.Net.Http
open System.Threading.Tasks
open System.Reactive.Linq

open AngleSharp
open AngleSharp.Browser
open AngleSharp.Dom

open FSharp.Control.Tasks.V2

/// 少女之心(曼娜回忆录)
type MannaTest(output: ITestOutputHelper) =
    let notebook = @"C:\Users\cuishengli\source\repos\xp44mm\Notebook"
    //下載的文件保存的目錄
    let htmlFileDir = Path.Combine(notebook, "曼娜回忆录")

    //[<Fact>]
    member this. ``下載曼娜回忆录`` () =
        let urlFormat = @"失效啦"

        //删除目录下的所有文件
        Directory.GetFiles(htmlFileDir)
        |> Array.iter(fun f -> File.Delete(f))

        let tcs = TaskCompletionSource<string>()

        let pages = [|1..6|]

        pages
            .ToObservable()
            .Select(fun ii ->
                task {
                    let client = new HttpClient()
                    let! response =
                        sprintf "%s%d" urlFormat ii
                        |> client.GetByteArrayAsync
                    let content = GettingStarted.GB18030.GetString response

                    let fileName = Path.Combine(htmlFileDir,ii.ToString()+".html")
                    do! File.WriteAllTextAsync(fileName,content,Encoding.UTF8)
                }
            )
            .Merge()
            //.Synchronize()
            //.ObserveOn(SynchronizationContext.Current)
            .Subscribe(
                (fun () ->()),
                (fun () ->
                    output.WriteLine("done!")
                    tcs.SetResult("done!"))
            )
        |> ignore

        tcs.Task

    [<Fact>]
    member this. ``norm`` () =
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
        
        let source = htmlFileDir
        let targetDir = Path.Combine(source, "target")

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
