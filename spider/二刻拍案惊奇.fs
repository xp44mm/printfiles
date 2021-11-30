namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Threading.Tasks
open System.Text
open System.Text.RegularExpressions
open System.Reactive.Linq
open System.Net.Http

open FSharp.Control.Tasks.V2

type 二刻拍案惊奇(output: ITestOutputHelper) =
    let hanchuancaolu = @"D:\githubrepos\xp44mm\hanchuancaolu"

    //[<Fact>]
    member this. ``下载`` () =        
        let rootDir = Path.Combine(hanchuancaolu, this.GetType().Name)

        //准备文件目录，删除目录下的所有文件
        Directory.GetFiles(rootDir)
        |> Array.iter(fun f -> File.Delete f )

        let tcs = TaskCompletionSource<string>()

        let website = "https://www.thn21.com/wen/famous/rlws/ekpa{0}.htm"
        let pages = [|1..40|]

        pages
            .ToObservable()
            .Select(fun ii ->
                task {
                    let client = new HttpClient()
                    let! response =
                        String.Format(website,ii)
                        |> client.GetByteArrayAsync
                    let content = GettingStarted.GB18030.GetString response

                    let fileName = 
                        Path.Combine(rootDir,String.Format("{0:00}.html",ii))
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

    //[<Fact>]
    member this. ``生成文本文件`` () =
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
                    let! src = File.ReadAllTextAsync file

                    let keep =
                        src.Replace(" ","whitespace")
                            .Replace("\n","newline")

                    let norm =
                        Regex.Replace(keep, @"\s+", "")
                            .Replace("whitespace"," ")
                            .Replace("newline","\n")

                    let targetFileName = 
                        Path.GetFileName(file)
                        |> fun file -> Path.Combine(target,file)
                    do! File.WriteAllTextAsync(targetFileName,norm)
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
