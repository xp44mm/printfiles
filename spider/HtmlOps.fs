module spider.HtmlOps

open FSharp.HTML

let rec pickParagraph (node:HtmlNode) =
    [
        match node with
        | HtmlElement(("h1"|"h2"|"h3"|"h4"|"h5"|"h6"|"p"|"span"),_,_) ->
            yield node
        | HtmlElement(_,_,[])-> ()
        | HtmlElement(_,_,nodes)->
            yield! 
                nodes
                |> List.collect pickParagraph
        | _ -> ()
    ]
       
let filterNode name (node:HtmlNode) =
    match node with
    | HtmlElement(_,_,[HtmlText x]) ->
        x <> name
    | _ -> true

let rec headinglist (nodes:HtmlNode list ) =
    match nodes with
    | HtmlElement(_,_,nodes)::t ->
        HtmlElement("h2",[],nodes)::t
    | _ -> nodes

let mergeSpans (nodes:HtmlNode list) =
    let rec loop (nodes:HtmlNode list) =
        seq {
            match nodes with
            | h::t ->
                match h with
                | HtmlElement(_,_,children) ->
                    yield! loop children
                | HtmlText x ->
                    yield x
                | _ -> failwithf "%A" h
                yield! loop t
            | [] -> ()
        }
    loop nodes
    |> String.concat ""

let renderPara = function
    | HtmlElement(("h1"|"h2"|"h3"|"h4"|"h5"|"h6"|"p"),_,nodes) ->
        mergeSpans nodes
    | node -> failwithf "%A" node

let normalize name text =
    text
    |> HtmlUtils.parseNodes
    |> List.collect pickParagraph
    |> List.filter (filterNode name)
    |> List.filter (filterNode "校對語譯：luo")
    |> headinglist
    |> List.map(function
        | HtmlElement(nm,attrs,children) ->
            let str = mergeSpans children
            HtmlElement(nm,attrs,[HtmlText str])
        | node -> node 
    )


open Xunit.Abstractions
open System
open System.IO
open System.Threading.Tasks
open System.Reactive.Linq

open FSharp.Control.Tasks.V2
open FSharp.Idioms

let writeToFiles (output: ITestOutputHelper) subfolder extname getContent =
    let source = Path.Combine(Dir.hanchuancaolu, subfolder)
    let target = Path.Combine("D:\\汉川草庐", subfolder)

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
                let content = getContent text
                //return nodes
                let targetFileName = 
                    basename + "." + extname
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
