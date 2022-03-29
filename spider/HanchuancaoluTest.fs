namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Threading.Tasks
open System.Reactive.Linq

open FSharp.Control.Tasks.V2
open FSharp.HTML
open FSharp.Literals
open System.Reflection

type HanchuancaoluTest(output: ITestOutputHelper) =

    let writeToFiles subfolder =
        let source = Path.Combine(Dir.hanchuancaolu, subfolder)
        let target = Path.Combine("D:", subfolder)

        //删除目标目录下所有文件
        Directory.GetFiles(target)
        |> Array.iter(File.Delete)

        let tcs = TaskCompletionSource<unit>()
        Directory
            .GetFiles(source)
            .ToObservable()
            .SelectMany(fun file ->
                task {
                    let basename = Path.GetFileNameWithoutExtension(file)
                    let! text = File.ReadAllTextAsync(file)
                    let nodes = HtmlOps.normalize subfolder text
                    let content =
                        nodes
                        |> List.map HtmlUtils.stringifyNode
                        |> String.concat "\r\n"
                    //return nodes
                    let targetFileName = 
                        basename + ".html"
                        |> fun file -> Path.Combine(target,file)

                    do! File.WriteAllTextAsync(targetFileName,content)

                })
            .Subscribe({
                new IObserver<_> with
                    member this.OnNext(ls) = ()
                    member this.OnError err = ()
                    member this.OnCompleted() = 
                        output.WriteLine("done!")
                        tcs.SetResult()
                })
        |> ignore
        tcs.Task

    [<Fact(Skip="done!")>]
    member this.``西遊記`` () =
        let subfolder = nameof this.西遊記
        writeToFiles subfolder

    [<Fact(Skip="done!")>]
    member this.``三國演義`` () =
        let subfolder = nameof this.三國演義
        writeToFiles subfolder

    [<Fact(Skip="done!")>]
    member this.``紅樓夢`` () =
        let subfolder = nameof this.紅樓夢
        writeToFiles subfolder

    [<Fact(Skip="done!")>]
    member this.``水滸傳`` () =
        let subfolder = nameof this.水滸傳
        writeToFiles subfolder

    [<Fact(Skip="done!")>]
    member this.``儒林外史`` () =
        let subfolder = nameof this.儒林外史
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``聊齋志異`` () =
        let subfolder = nameof this.聊齋志異
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``老子`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``莊子`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``荀子`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``墨子`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``孫子`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    ///十三经
    [<Fact(Skip="done!")>] //
    member this.``易經`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``尚書`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``詩經`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``周禮`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``儀禮`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``禮記`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``左傳`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``公羊傳`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``穀梁傳`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``論語`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``孝經`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact(Skip="done!")>] // 
    member this.``爾雅`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder

    [<Fact>] // (Skip="done!")
    member this.``孟子`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        writeToFiles subfolder





