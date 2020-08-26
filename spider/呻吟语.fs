namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Threading.Tasks
open System.Reactive.Linq
open System.Net.Http

open FSharp.Control.Tasks.V2

open AngleSharp
open AngleSharp.Dom


//http://www.chang-lian.cn/shuji_detail/19627.html
//在命令行执行以下命令复制
//document.querySelector('div.main-content.gushi-info')
//copy($_)
type 呻吟语(output: ITestOutputHelper) =
    let hanchuancaolu = @"d:\source\repos\xp44mm\hanchuancaolu"

    //[<Fact>]
    member this. ``下载`` () =        
        let rootDir = Path.Combine(hanchuancaolu, this.GetType().Name)

        //准备文件目录，删除目录下的所有文件
        Directory.GetFiles(rootDir)
        |> Array.iter(fun f -> File.Delete f )

        let tcs = TaskCompletionSource<string>()

        let website = "http://www.chang-lian.cn/shuji_detail/19627.html"
        let pages = 
            [|"序"; "礼集·性命"; "礼集·存心"; "礼集·伦理"; "礼集·谈道"; "乐集·修身"; "乐集·问学"; "射集·应务"; "射集·养生"; "御集·天地"; "御集·世运"; "御集·圣贤"; "御集·品藻"; "书集·治道"; "数集·人情"; "数集·物理"; "数集·广喻"; "数集·词章"; |]
            |> Array.mapi(fun i name -> i,name)
        pages
            .ToObservable()
            .Select(fun (ii,title) ->
                task {
                    let client = new HttpClient()
                    let! content = 
                        String.Format(website,19627+ii)
                        |> client.GetStringAsync

                    let fileName = 
                        Path.Combine(rootDir,String.Format("{0:00} {1}.html",ii,title))
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

    [<Fact>]
    member this. ``生成文本文件`` () =

        //假设没有内联节点元素，全部都是块元素。
        let getTexts (root:INode) =
            let rec loop (texts:string list) (node:INode) =
                [
                    yield! texts
                    if node.NodeType = NodeType.Element then
                        for node in node.ChildNodes do
                            yield! loop [] node
                    elif node.NodeType = NodeType.Text then
                        let txt = (node:?>IText).Text
                        if String.IsNullOrWhiteSpace(txt) then
                            ()
                        else yield txt
                    else failwith ""
                ]

            loop [] root

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
