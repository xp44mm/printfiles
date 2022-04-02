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

type HtmlToTextTest(output: ITestOutputHelper) =
    let getContent text =
        let nodes = HtmlUtils.parseNodes text
        let content =
            nodes
            |> List.map HtmlOps.renderPara
            |> String.concat "\r\n"
        content

    [<Fact>] // (Skip="done!")
    member this.``三國演義`` () =
        let subfolder = MethodBase.GetCurrentMethod().Name
        HtmlOps.writeToFiles output subfolder "txt" getContent


