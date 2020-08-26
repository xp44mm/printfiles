namespace spider

open Xunit
open Xunit.Abstractions
open System
open System.Reflection
open System.IO
open System.Reactive.Linq
open System.Threading.Tasks
open System.Text.RegularExpressions

///重命名文件，全角空白變爲ASCII空格
type HanchuancaoluRenameTest(output: ITestOutputHelper) =
    let hanchuancaolu = @"d:\source\repos\xp44mm\hanchuancaolu"

    let renamefile (currentMethod : MethodBase) =
        let tcs = TaskCompletionSource<string>()
        let folder = Path.Combine(hanchuancaolu,currentMethod.Name)
        
        let files =
            folder
            |> Directory.GetFiles
        files
            .ToObservable()
            .Select(fun file ->
                let oldfile = Path.GetFileNameWithoutExtension(file)
                let newfile = Regex.Replace(oldfile.Trim(),@"\s+"," ")
                (oldfile,newfile)
            )
            .Where(fun (oldfile,newfile) ->
                oldfile <> newfile
            )
            .Select(fun(oldfile,newfile) ->
                Path.Combine(folder,oldfile)+".html",Path.Combine(folder,newfile)+".html")
            .Synchronize()
            .Subscribe(
                (fun(oldfile,newfile) ->
                    //output.WriteLine(newfile)
                    let fi = new FileInfo(oldfile)
                    fi.MoveTo(newfile)
                ),
                (fun()-> 
                    tcs.SetResult("done!")
                    output.WriteLine("done!"))
                )
            |> ignore
        tcs.Task

    ///[<Fact>]
    member this. ``紅樓夢`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``水滸傳`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``西遊記`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod


    ///[<Fact>]
    member this. ``三國演義`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``聊齋志異`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``儒林外史`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``老子`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``莊子`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``荀子`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``墨子`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``孫子`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///十三经
    ///[<Fact>]
    member this. ``易經`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``尚書`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``詩經`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``周禮`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``儀禮`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``禮記`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``左傳`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``公羊傳`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``穀梁傳`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod


    ///[<Fact>]
    member this. ``論語`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod


    ///[<Fact>]
    member this. ``孝經`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``爾雅`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

    ///[<Fact>]
    member this. ``孟子`` () =
        let currentMethod = MethodBase.GetCurrentMethod()
        renamefile currentMethod

