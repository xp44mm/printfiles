namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Reflection
open System.Reactive.Linq
open System.Threading.Tasks

open FSharp.Control.Tasks.V2

open AngleSharp

///“三言”即《喻世明言》《警世通言》《醒世恒言》的合称。
type 三言生成文本文件(output: ITestOutputHelper) =
    let htmlToTxt subdir =
        let sourceDir = Path.Combine(Dir.hanchuancaolu, subdir)
        let targetDir = Path.Combine(@"D:\三言", subdir)

        //删除目标目录下所有文件
        Directory.GetFiles(targetDir)
        |> Array.iter(File.Delete)

        let tcs = TaskCompletionSource<string>();

        Directory.GetFiles(sourceDir)
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
                        let file = Path.GetFileNameWithoutExtension(file)+".txt"
                        Path.Combine(targetDir,file)
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

    [<Fact>]
    member this. ``喻世明言`` () =
        htmlToTxt <| MethodBase.GetCurrentMethod().Name

    [<Fact>]
    member this. ``警世通言`` () =
        htmlToTxt <| MethodBase.GetCurrentMethod().Name

    [<Fact>]
    member this. ``醒世恒言`` () =
        htmlToTxt <| MethodBase.GetCurrentMethod().Name

