namespace spider

open Xunit
open Xunit.Abstractions

open System.Text
open System.Net.Http
open System.Reflection
open System.IO
open System
open System.Threading.Tasks
open System.Reactive.Linq

open FSharp.Control.Tasks.V2

type 初刻拍案惊奇(output: ITestOutputHelper) =
    let hanchuancaolu = @"C:\Users\cuishengli\source\repos\xp44mm\hanchuancaolu"

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


