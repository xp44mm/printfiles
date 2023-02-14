namespace spider

open Xunit
open Xunit.Abstractions

open System.Reflection

open FSharp.HTML
open FSharp.Idioms

type HtmlToTextTest(output: ITestOutputHelper) =
    let getContent text =
        let nodes = 
            text
            |> HtmlUtils.parseDoc
            |> snd

        let content =
            nodes
            |> List.map HtmlOps.renderPara
            |> List.append <| [""]
            |> String.concat "\r\n"
        content




    [<Fact>] // (Skip="done!")
    member this.``封神演义`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``西遊記`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``三國演義`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``水滸傳`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``紅樓夢`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``儒林外史`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``喻世明言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``警世通言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``醒世恒言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``初刻拍案惊奇`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact>] // (Skip="done!")
    member this.``二刻拍案惊奇`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent





