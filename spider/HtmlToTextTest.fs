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
            |> String.concat "\r\n"
        content

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


