namespace spider

open Xunit
open Xunit.Abstractions

open System.Text
open System.Net.Http
open System.IO
open System
open System.Threading.Tasks
open System.Reactive.Linq

open FSharp.Control.Tasks.V2
open AngleSharp.Dom
open AngleSharp

type 初刻拍案惊奇(output: ITestOutputHelper) =
    let hanchuancaolu = @"d:\xp44mm\hanchuancaolu"

    [<Fact>]
    member this. ``生成文本文件`` () =

        //假设没有内联节点元素，全部都是块元素。
        let getTexts (root:IElement) =
            [
                yield root.GetElementsByTagName("h2").[0].GetElementsByTagName("font").[0].TextContent.Trim()
                for element in root.GetElementsByTagName("p") do
                    let txt = element.TextContent
                    yield txt.Trim()
            ]

        let source = Path.Combine(hanchuancaolu, this.GetType().Name)
        let target = Path.Combine(source, "text")

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
                        getTexts document.Body
                        |> String.concat "\n"

                    let targetFileName =
                        Path.GetFileNameWithoutExtension(file)
                        |> fun file -> Path.Combine(target, file+".txt")
                    do! File.WriteAllTextAsync(targetFileName, content)
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


    //[<Fact>]
    member this. ``初刻拍案惊奇`` () =
        let dir = Path.Combine(hanchuancaolu, this.GetType().Name)

        //准备文件目录，删除目录下的所有文件
        Directory.GetFiles(dir)
        |> Array.iter(fun f -> File.Delete f )

        let tcs = TaskCompletionSource<string>()

        let website = "https://www.kanunu8.com/files/old/2011/2528/{0}.html"
        let pages = [|0..39|]

        pages
            .ToObservable()
            .Select(fun ii ->
                task {
                    let client = new HttpClient()
                    let! response =
                        String.Format(website,74808+ii)
                        |> client.GetByteArrayAsync
                    let content = GettingStarted.GB18030.GetString response

                    let fileName = Path.Combine(dir,ii.ToString()+".html")
                    do! File.WriteAllTextAsync(fileName,content,Encoding.UTF8)
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


