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
            //|> snd

        let content =
            nodes
            |> List.map HtmlOps.renderPara
            |> List.append <| [""]
            |> String.concat "\r\n"
        content

    [<Fact
     // (Skip="done!")
    >]
    member this.``01-封神演义`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``02-西遊記`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``03-三國演義`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``04-水滸傳`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
    // (Skip="done!")
    >] 
    member this.``05-紅樓夢`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``06-儒林外史`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``07-喻世明言`` () =
        //let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output "喻世明言" "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``08-警世通言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``09-醒世恒言`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``10-初刻拍案惊奇`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent

    [<Fact
     // (Skip="done!")
    >]
    member this.``11-二刻拍案惊奇`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent





