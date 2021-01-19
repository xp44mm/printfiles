namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Threading.Tasks
open System.Text.RegularExpressions
open System.Reactive.Linq
open System.Net.Http
open FSharp.Control.Tasks.V2

open AngleSharp
open AngleSharp.Dom
open AngleSharp.Browser
open System.Reflection

//在控制台执行以下命令，复制html节点到剪贴板
//copy(document.querySelector('div.chapter-content-wrap'))

/// 内容来自百度小说：http://dushu.baidu.com/
/// 生成当地的html文件
type 三言(output: ITestOutputHelper) =
    let removeBr (e:IElement) =
        e.ChildNodes
        |> Seq.map(fun node-> node.Clone())
        |> Seq.filter(fun node -> node.NodeType = NodeType.Text)
        |> Seq.map(fun node ->
            let p = e.Owner.CreateElement("P")
            p.AppendChild node |> ignore
            p
        )

    let convertToHtml subdir =
        let sourceDir = Path.Combine(Path.Combine(@"D:\三言", subdir),"download")
        let targetDir = Path.Combine(Dir.hanchuancaolu, subdir)

        //*****删除目录下的所有文件******
        Directory.GetFiles(targetDir)
        |> Array.iter(fun f -> File.Delete(f))

        let tcs = TaskCompletionSource<string>()
        let files = 
            Directory.GetFiles(sourceDir)
            |> Array.mapi(fun i file -> i, file)

        files
            .ToObservable()
            .Select(fun (i,file) ->
                task {
                    let! text = File.ReadAllTextAsync(file)
                    let context = BrowsingContext.New(Configuration.Default)

                    let! document = 
                        context.OpenAsync(fun req -> req.Content(text) |> ignore)

                    let title = document.QuerySelector(".chapter-title")

                    let container = 
                        document.QuerySelector("div.chapter-content")

                    let content = 
                        removeBr container
                        |> Seq.map(fun e -> e.OuterHtml)
                        |> String.concat "\n"

                    let targetFileName =
                        let file = String.Format("{0:00} {1}.html", i+1, title.ChildNodes.[0].Text())
                        Path.Combine(targetDir,file)

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
    member this. ``喻世明言`` () =
        convertToHtml <| MethodBase.GetCurrentMethod().Name

    //[<Fact>]
    member this. ``警世通言`` () =
        convertToHtml <| MethodBase.GetCurrentMethod().Name

    //[<Fact>]
    member this. ``醒世恒言`` () =
        convertToHtml <| MethodBase.GetCurrentMethod().Name


