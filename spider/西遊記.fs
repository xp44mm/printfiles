namespace spider

open Xunit
open Xunit.Abstractions

open System
open System.IO
open System.Threading.Tasks
open System.Reactive.Linq

open FSharp.Control.Tasks.V2
open FSharp.HTML // usage first
open FSharp.Literals

type 西遊記(output: ITestOutputHelper) as this =
    let hanchuancaolu = Path.Combine(Dir.hanchuancaolu, this.GetType().Name)
    
    [<Fact>]
    member this. ``all of tag names`` () =
        let tcs = TaskCompletionSource<string>();
        let arr = ResizeArray<string>()
        Directory.GetFiles(hanchuancaolu)
            .ToObservable()
            .SelectMany(fun file ->
                task {
                    let! text = File.ReadAllTextAsync(file)

                    let nodes = Parser.parseNodes text

                    let rec loop ls nodes =
                        match nodes with
                        | [] -> ls
                        | node:HtmlNode::tail ->
                            let ls = 
                                match node with
                                | HtmlElement(name,_,_) -> 
                                    name::ls
                                | _ -> ls
                            loop ls tail
                    return loop [] nodes
                }
            )
            .Subscribe(
                (fun ls -> 
                    arr.AddRange(ls)|> ignore),
                (fun () -> 
                    let x =
                        arr.ToArray()
                        |> Array.distinct
                        |> Array.toList

                    output.WriteLine(Literal.stringify x)
                    tcs.SetResult("done!"))
            )
        |> ignore

        tcs.Task
