namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Reflection
open System.Threading.Tasks

open System.Reactive.Linq
open FSharp.Control.Tasks.V2
open AngleSharp
open System.Text.RegularExpressions

type HanchuancaoluToTextTest(output: ITestOutputHelper) =
    let hanchuancaolu = @"d:\source\repos\xp44mm\hanchuancaolu"

    /// get parsed document
    let parseFileAsync file =
        task {
            let context = BrowsingContext.New(Configuration.Default)
            let! text = File.ReadAllTextAsync(file)
            let! document = context.OpenAsync(fun req -> req.Content(text) |> ignore)
            return document
        }

    let htmlToTextAsync folder convert =
        let source = Path.Combine(hanchuancaolu,folder)
        let target = Path.Combine("c:", folder)

        //删除目标目录下所有文件
        Directory.GetFiles(target)
        |> Array.iter(File.Delete)

        let tcs = TaskCompletionSource<string>();

        Directory.GetFiles(source)
            .ToObservable()
            .Select(fun file ->
                task {
                    let! document = parseFileAsync file      
                    let content = convert document
                    let targetFileName = 
                        Path.GetFileNameWithoutExtension(file) + ".txt"
                        |> fun file -> Path.Combine(target,file)
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
    member this. ``紅樓夢`` () =
        let folder = MethodBase.GetCurrentMethod().Name

        htmlToTextAsync folder (fun document ->
            document.Body.Children
            |> Seq.skipWhile(fun elem -> elem.TagName <> "H2")
            |> Seq.takeWhile(fun elem -> elem.TagName <> "HR")
            |> Seq.map(fun elem -> elem.TextContent.Trim())
            |> String.concat "\n"
        )
    [<Fact>]
    member this. ``西遊記`` () =
        let folder = MethodBase.GetCurrentMethod().Name

        htmlToTextAsync folder (fun document ->
            document.Body.Children
            |> Seq.skipWhile(fun elem -> elem.TagName <> "H2")
            |> Seq.takeWhile(fun elem -> elem.TagName <> "HR")
            |> Seq.map(fun elem -> elem.TextContent.Trim())
            |> String.concat "\n"
        )

    [<Fact>]
    member this. ``水滸傳`` () =
        let folder = MethodBase.GetCurrentMethod().Name

        htmlToTextAsync folder (fun document ->
            document.Body.Children
            |> Seq.skipWhile(fun elem -> elem.TagName <> "H2")
            |> Seq.takeWhile(fun elem -> elem.TagName <> "HR")
            |> Seq.map(fun elem -> elem.TextContent.Trim())
            |> String.concat "\n"
        )

    [<Fact>]
    member this. ``三國演義`` () =
        let folder = MethodBase.GetCurrentMethod().Name

        htmlToTextAsync folder (fun document ->
            document.Body.Children
            |> Seq.skipWhile(fun elem -> elem.TagName <> "H2")
            |> Seq.takeWhile(fun elem -> elem.TagName <> "HR")
            |> Seq.map(fun elem -> elem.TextContent.Trim())
            |> String.concat "\n"
        )

    [<Fact>]
    member this. ``儒林外史`` () =
        let folder = MethodBase.GetCurrentMethod().Name

        htmlToTextAsync folder (fun document ->
            document.Body.Children
            |> Seq.skipWhile(fun elem -> elem.TagName <> "H2")
            |> Seq.takeWhile(fun elem -> elem.TagName <> "HR")
            |> Seq.map(fun elem -> elem.TextContent.Trim())
            |> String.concat "\n"
        )

    [<Fact>]
    member this. ``聊齋志異`` () =
        let folder = MethodBase.GetCurrentMethod().Name

        htmlToTextAsync folder (fun document ->
            document.Body.Children
            |> Seq.filter(fun elem -> elem.TagName <> "H5")
            |> Seq.filter(fun elem -> elem.TagName <> "HR")
            |> Seq.map(fun elem -> elem.TextContent.Trim())
            |> String.concat "\n"
        )

    [<Fact>]
    member this. ``初刻拍案惊奇`` () =
        let folder = MethodBase.GetCurrentMethod().Name

        htmlToTextAsync folder (fun document ->
            document.Body.Children
            |> Seq.map(fun elem -> elem.TextContent.Trim())
            |> String.concat "\n"
        )
