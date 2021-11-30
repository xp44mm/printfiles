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

type 封神演义(output: ITestOutputHelper) =
    let hanchuancaolu = @"D:\githubrepos\xp44mm\hanchuancaolu"

    //[<Fact>]
    member this. ``下载`` () =
        let rootDir = Path.Combine(hanchuancaolu, this.GetType().Name)

        //准备文件目录，删除目录下的所有文件
        Directory.GetFiles(rootDir)
        |> Array.iter(fun f -> File.Delete f )

        let tcs = TaskCompletionSource<string>()

        let website = "http://dushu.baidu.com/pc/reader?gid=4306339995&cid="
        let pages = [|0..99|]

        pages
            .ToObservable()
            .Select(fun ii ->
                task {
                    let client = new HttpClient()
                    let! content =
                        website+(11424835+ii).ToString()
                        |> client.GetStringAsync

                    let fileName =
                        Path.Combine(rootDir,String.Format("{0:000}.html",ii))
                    do! File.WriteAllTextAsync(fileName,content)
                }
            )
            .Merge()
            .Subscribe(
                (fun () ->()),
                (fun () ->
                    output.WriteLine("done!")
                    tcs.SetResult("done!"))
            )
        |> ignore

        tcs.Task

    //[<Fact>]
    member this. ``norm remove br`` () =
        
        let removeBr (e:IElement) =
            e.ChildNodes
            |> Seq.map(fun node-> node.Clone())
            |> Seq.filter(fun node -> node.NodeType = NodeType.Text)
            |> Seq.map(fun node ->
                let p = e.Owner.CreateElement("P")
                p.AppendChild node |> ignore
                p
            )

        let rootDir = Path.Combine(hanchuancaolu, this.GetType().Name)
        let source = Path.Combine(rootDir,"download")
        let targetDir = Path.Combine(rootDir, "target")

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
                        document.QuerySelector("div.chapter-content")


                    let content = //removeBr container.OuterHtml
                        removeBr container
                        |> Seq.map(fun e -> e.OuterHtml)
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
    member this. ``修改文件名`` () =
        let source = Path.Combine(hanchuancaolu, this.GetType().Name)
        let target = Path.Combine(source, "target")

        //删除目标目录下所有文件       
        Directory.GetFiles(target)
        |> Array.iter(File.Delete)

        let tcs = TaskCompletionSource<string>();

        Directory.GetFiles(source)
            .ToObservable()
            .Select(fun file ->
                task {
                    let context = 
                        BrowsingContext.New(Configuration.Default)
                    let! text = File.ReadAllTextAsync(file)
                    let! document = context.OpenAsync(fun req -> req.Content(text) |> ignore)

                    let targetFileName = 
                        [
                            Path.GetFileNameWithoutExtension(file)
                            document.Body.Children.[0].TextContent
                            document.Body.Children.[1].TextContent
                        ] |> String.concat " "


                    let targetFileName =                    
                        Path.Combine(target,targetFileName + ".html")
                    do! File.WriteAllTextAsync(targetFileName,document.Body.InnerHtml)
                }
            )
            .Merge()
            .Subscribe(
                (fun () -> ()),
                (fun () ->
                    output.WriteLine("done!")
                    tcs.SetResult("done!"))
            )
        |> ignore

        tcs.Task

    [<Fact>]
    member this. ``生成文本文件`` () =
        let folder = this.GetType().Name
        let source = Path.Combine(hanchuancaolu, folder)
        let target = Path.Combine("D:", folder)

        //删除目标目录下所有文件
        Directory.GetFiles(target)
        |> Array.iter(File.Delete)

        let tcs = TaskCompletionSource<string>();

        Directory.GetFiles(source)
            .ToObservable()
            .Select(fun file ->
                task {
                    let! text = File.ReadAllTextAsync(file)
                    let context = BrowsingContext.New(Configuration.Default)

                    let! document =
                        context.OpenAsync(fun req -> req.Content(text) |> ignore)

                    let content =
                        document.Body.Children
                        |> Seq.map(fun e -> e.TextContent.Trim())
                        |> String.concat "\n"

                    let targetFileName = 
                        Path.GetFileNameWithoutExtension(file)
                        |> fun file -> Path.Combine(target,file+".txt")
                    do! File.WriteAllTextAsync(targetFileName,content)
                }
            )
            .Merge()
            .Subscribe(
                (fun () -> ()),
                (fun () -> 
                    output.WriteLine("done!")
                    tcs.SetResult("done!"))
            )
        |> ignore

        tcs.Task
