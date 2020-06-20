namespace spider

open Xunit
open Xunit.Abstractions
open System
open System.Reflection
open System.IO
open System.Reactive.Linq
open System.Threading.Tasks
open System.Net.Http

open FSharp.Control.Tasks.V2

type HanchuancaoluDownloadTest(output: ITestOutputHelper) =
    let website = @"http://www.xn--5rtnx620bw5s.tw"
    let repo = @"C:\hanchuancaolu"

    ///用前導零對齊數字
    let numArrayFormat (pages:int[]) =
        let numFormat = String.Format("D{0}",pages.Length.ToString().Length)
        pages
        |> Array.map(fun i -> i.ToString(numFormat) )

    let crawler (folder:string) (getUrl:string -> string) (pages:string[]) =
        let dir = Path.Combine(repo, folder)

        ////删除目录下的所有文件
        //Directory.GetFiles(dir)
        //|> Array.iter(fun f -> File.Delete(f))

        let tcs = TaskCompletionSource<string>();

        pages
            .ToObservable()
            .Select(fun ii ->
                task {
                    let client = new HttpClient()
                    let! content = client.GetStringAsync (getUrl ii)
                    let fileName = Path.Combine(dir,ii+".html")
                    do! File.WriteAllTextAsync(fileName,content)
                    //return ii
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

    ///[<Fact>]
    member this. ``紅樓夢`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/e/e1/{0}.htm",ii)
        let pages = numArrayFormat [|1..120|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``水滸傳`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl ii = String.Format("{0}/e/e2/{1}.htm",website, ii)
        let pages = numArrayFormat [|1..120|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``西遊記`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl ii = String.Format("{0}/e/e3/{1}.htm",website, ii)
        let pages = numArrayFormat [|1..100|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``三國演義`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl ii = String.Format("{0}/e/e5/{1}.htm",website, ii)
        let pages = numArrayFormat [|1..120|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``聊齋志異`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl ii = String.Format("{0}/e/e6/{1}.htm",website, ii)
        let pages =
            numArrayFormat [|1..498|]
            |> Array.append [| "e06" |] // ``聊齋志異：自序``
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``儒林外史`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/e/e7/0{0}.htm", ii)
        let pages = numArrayFormat [|1..55|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``老子`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/h/h01/0{0}.htm", ii)
        let pages = numArrayFormat [|1..9|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``莊子`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/d/{0}.htm", ii)
        let pages = numArrayFormat [|1..33|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``荀子`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/h/h02/{0}.htm", ii)
        let pages = numArrayFormat [|1..32|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``墨子`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/h/h03/{0}.htm", ii)
        let pages = numArrayFormat [|1..51|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``孫子`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/h/h04/0{0}.html", ii)
        let pages = numArrayFormat [|1..13|]
        crawler folder getUrl pages

    ///十三经
    ///[<Fact>]
    member this. ``易經`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b01/0{0}.htm", ii)
        let pages = numArrayFormat [|1..69|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``尚書`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b02/0{0}.htm", ii)
        let pages = numArrayFormat [|1..59|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``詩經`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b03/0{0}.htm", ii)
        let pages = numArrayFormat [|1..31|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``周禮`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b04/0{0}.htm", ii)
        let pages = numArrayFormat [|1..6|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``儀禮`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b05/{0}.htm", ii)
        let pages = numArrayFormat [|1..17|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``禮記`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b06/{0}.htm", ii)
        let pages = numArrayFormat [|1..49|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``左傳`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b07/{0}.htm", ii)
        let pages = numArrayFormat [|1..255|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``公羊傳`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b08/{0}.htm", ii)
        let pages = numArrayFormat [|1..242|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``穀梁傳`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b09/{0}.htm", ii)
        let pages = numArrayFormat [|1..242|]
        crawler folder getUrl pages


    ///[<Fact>]
    member this. ``論語`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b10/{0}.htm", ii)
        let pages = numArrayFormat [|1..20|]
        crawler folder getUrl pages


    ///[<Fact>]
    member this. ``孝經`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b11.htm")
        let pages = [|"0"|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``爾雅`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b12/{0}.htm", ii)
        let pages = numArrayFormat [|1..19|]
        crawler folder getUrl pages

    ///[<Fact>]
    member this. ``孟子`` () =
        let folder = MethodBase.GetCurrentMethod().Name
        let getUrl (ii:string) = website + String.Format("/b/b13/{0}.htm", ii)
        let pages = numArrayFormat [|1..14|]
        crawler folder getUrl pages

