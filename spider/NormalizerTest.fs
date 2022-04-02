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
open FSharp.Idioms

type NormalizerTest(output: ITestOutputHelper) =

    [<Fact(Skip="done!")>] // 
    member this.``初刻拍案惊奇`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        let getContent text =
            let nodes =
                HtmlOps.normalize subfolder text
            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"

            content

        HtmlOps.writeToFiles output subfolder "html" getContent

    [<Fact(Skip="done!")>] // 
    member this.``二刻拍案惊奇`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        let getContent (text:string) =
            let nodes = 
                text
                |> Line.splitLines
                |> Seq.choose(fun (i, line:string) ->
                    match line.Trim() with
                    | "" -> None
                    | x -> 
                        let tag = if i = 0 then "h2" else "p"
                        HtmlElement(tag,[],[HtmlText x]) |> Some
                )
        
            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"
        
            content
        HtmlOps.writeToFiles output subfolder "html" getContent

    [<Fact(Skip="done!")>] // 
    member this.``封神演义`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        let getContent (text:string) =
            let nodes = 
                match HtmlUtils.parseNodes text with
                | HtmlElement(_,_,[HtmlText a])::HtmlElement(_,_,[HtmlText b])::nodes ->
                    let h = HtmlElement("h2",[],[HtmlText $"{a}：{b}"])
                    h::nodes
                | x -> failwithf "%A" x

            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"
        
            content
        HtmlOps.writeToFiles output subfolder "html" getContent

    [<Fact>] // (Skip="done!")
    member this.``儒林外史`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        //只是为了格式化空格
        let getContent text =
            let nodes = 
                text
                |> HtmlUtils.parseNodes

            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"

            content

        HtmlOps.writeToFiles output subfolder "html" getContent

    [<Fact>] //(Skip="done!")
    member this.``呻吟语`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        let getContent (text:string) =
            let nodes = 
                text
                |> HtmlUtils.parseNodes
                |> List.collect HtmlOps.pickParagraph
                |> List.map(function
                    | HtmlElement(nm,attrs,children) ->
                        let str = HtmlOps.mergeSpans children
                        HtmlElement(nm,attrs,[HtmlText str])
                    | node -> node 
                )

            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"
        
            content
        HtmlOps.writeToFiles output subfolder "html" getContent
            
    [<Fact>] // (Skip="done!")
    member this.``警世通言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        let getContent (text:string) =
            let nodes = 
                match HtmlUtils.parseNodes text with
                | HtmlElement(_,_,[HtmlText a])::HtmlElement(_,_,[HtmlText b])::nodes ->
                    let h = HtmlElement("h2",[],[HtmlText $"{a}：{b}"])
                    h::nodes
                | x -> failwithf "%A" x

            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"
        
            content
        HtmlOps.writeToFiles output subfolder "html" getContent

    [<Fact>] // (Skip="done!")
    member this.``醒世恒言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        let getContent (text:string) =
            let nodes = 
                match HtmlUtils.parseNodes text with
                | HtmlElement(_,_,[HtmlText a])::HtmlElement(_,_,[HtmlText b])::nodes ->
                    let h = HtmlElement("h2",[],[HtmlText $"{a}：{b}"])
                    h::nodes
                | x -> failwithf "%A" x

            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"
        
            content
        HtmlOps.writeToFiles output subfolder "html" getContent

    [<Fact>] // (Skip="done!")
    member this.``喻世明言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        let getContent (text:string) =
            let nodes = 
                match HtmlUtils.parseNodes text with
                | HtmlElement(_,_,[HtmlText a])::HtmlElement(_,_,[HtmlText b])::nodes ->
                    let h = HtmlElement("h2",[],[HtmlText $"{a}：{b}"])
                    h::nodes
                | x -> failwithf "%A" x

            let content =
                nodes
                |> Seq.map HtmlUtils.stringifyNode
                |> String.concat "\r\n"
        
            content
        HtmlOps.writeToFiles output subfolder "html" getContent


