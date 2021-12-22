namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Threading.Tasks
open System.Reactive.Linq

open FSharp.Control.Tasks.V2

open AngleSharp
open AngleSharp.Dom


type 儒林外史(output: ITestOutputHelper) =

    [<Fact>]
    member this. ``生成文本文件`` () =

        //假设没有内联节点元素，全部都是块元素。
        let getTexts (root:IElement) =
            [
                yield root.GetElementsByTagName("h2").[0].Text()
                for element in root.GetElementsByTagName("p") do
                    let txt = element.Text()
                    yield txt.Trim()
            ]

        let source = Path.Combine(Dir.hanchuancaolu, this.GetType().Name)
        let target = Path.Combine(@"d:\", this.GetType().Name)

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
