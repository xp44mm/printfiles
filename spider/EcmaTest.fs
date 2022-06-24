namespace spider

open Xunit
open Xunit.Abstractions

open System.Reflection
open System.Text
open System.Text.RegularExpressions

open FSharp.HTML
open FSharp.Idioms
open System.IO

type EcmaTest(output: ITestOutputHelper) =


    [<Fact>]
    member _.``00 - defs``() =
        let clauseIds = set [
            "sec-ecmascript-language-expressions"
            "sec-ecmascript-language-statements-and-declarations"
            "sec-ecmascript-language-functions-and-classes"
            "sec-ecmascript-language-scripts-and-modules"
        ]
        let rec extractDefi node =
            seq {
                match node with
                | HtmlElement("emu-grammar",attrs,[HtmlText text]) ->
                    if attrs
                        |> List.exists(fun(n,v) ->
                            n="type" && v="definition"
                            )
                    then yield text
                    else ()
                | HtmlElement(_,_,children) ->
                    yield!
                        children
                        |> Seq.collect extractDefi
                | _ -> ()

            }

        let file = Path.Combine(Dir.ecma262,"spec.html")
        let text = File.ReadAllText(file)

        let nodes =
            text
            //|> Tokenizer.tokenize
            //|> HtmlTokenUtils.preamble
            //|> snd
            //|> Seq.choose HtmlTokenUtils.unifyVoidElement
            //|> Compiler.parse
            |> Parser.parseDoc
            |> snd
            |> List.map HtmlCharRefs.unescapseNode

        let clauses =
            nodes
            |> Seq.filter(function
                | HtmlElement("emu-clause",attrs,_) ->
                    attrs
                    |> List.exists(fun(n,v) -> 
                        n="id" && clauseIds.Contains v
                        )
                | _ -> false
            )

        let defs = 
            clauses
            |> Seq.collect extractDefi
            |> String.concat "\r\n"

        let filepath = Path.Combine(__SOURCE_DIRECTORY__, "defs.fsyacc")
        File.WriteAllText(filepath,defs,Encoding.UTF8)
        output.WriteLine("done!")

    [<Fact>] //(Skip="done!")
    member this.``01 - grammar fsyacc`` () =
        let inp =
            Path.Combine(__SOURCE_DIRECTORY__,"defs.fsyacc")
            |> fun f -> File.ReadAllText(f)
        let x =
            inp.Replace("[empty]","(*empty*)")
                .Replace("`","\"")
                .Replace("but not ReservedWord"," ")
            |> fun x -> Regex.Replace(x,@"(#|//).+","")
            |> fun x -> Regex.Replace(x,@"(?<!"")\[[^[""\]]+?\](?!"") *"," ")
            |> fun x -> Regex.Replace(x,@"(\[lookahead.*?\](?!""))( *)"," ")
            |> fun x -> Regex.Replace(x,@"(\S) +\?","$1?")
            |> fun x -> Regex.Replace(x,@"\s*one of\b","\r\n$0")

        let y =
            x
            |> fun x -> Regex.Split(x,@"\r?\n")
            |> Array.map(fun x -> x.Trim())
            |> Array.map(fun x ->
                if x = "" || x.EndsWith(':') then
                    x
                else
                    let x = Regex.Replace(x, @" {2,}", " ")
                    $"    | {x} {{}}"
            )
            |> String.concat "\r\n"

        let outpath = Path.Combine(Dir.xp44mm,
            @"JavaScriptCompiler\JavaScriptCompiler\fsyacc sources")
        let filepath = Path.Combine(outpath,"ecmascript.fsyacc")
        File.WriteAllText(filepath,y,Encoding.UTF8)
        output.WriteLine("done!")