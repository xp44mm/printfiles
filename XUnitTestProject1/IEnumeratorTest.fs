namespace Tests

open Xunit
open Xunit.Abstractions

open System
open System.IO
open FSharp.Literals

type IEnumeratorTest(output: ITestOutputHelper) =
    let show res = 
        res
        |> Literal.stringify
        |> output.WriteLine

    [<Fact>]
    member this.``enum``() =
        let ls = [0;1;2]
        let enumerator = (Seq.ofList ls).GetEnumerator()
        show <| enumerator.MoveNext()
        show enumerator.Current

        show <| enumerator.MoveNext()
        show enumerator.Current

        show <| enumerator.MoveNext()
        show enumerator.Current

        show <| enumerator.MoveNext()
        //show enumerator.Current
        show <| enumerator.MoveNext()
        show <| enumerator.MoveNext()


    [<Fact>]
    member this.``enum empty seq``() =
        let ls = []
        let enumerator = (Seq.ofList ls).GetEnumerator()
        show <| enumerator.MoveNext()
        show enumerator.Current

        //show <| enumerator.MoveNext()
        //show enumerator.Current

        //show <| enumerator.MoveNext()
        //show enumerator.Current

        //show <| enumerator.MoveNext()
        //show <| enumerator.MoveNext()
        //show <| enumerator.MoveNext()

