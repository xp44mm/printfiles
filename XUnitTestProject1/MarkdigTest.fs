namespace Tests

open Xunit
open Xunit.Abstractions

open System
open System.IO

open System.Text
open Markdig

open Markdig.Renderers.Html;
open Markdig.Syntax;
open Markdig.Syntax.Inlines;
open Markdig.Renderers.Normalize

module Markdigs =
    let GetTypeName (t:Type) = t.Name

    //let pipelineBuilder = MarkdownPipelineBuilder().UsePreciseSourceLocation();
    //if extensions <> null then
    //    pipelineBuilder.Configure(extensions);

    //let pipeline = pipelineBuilder.Build();


    //遍历文档
    let markdownType document =
        let build = new StringBuilder();
        for v:Block in document do
            let name = GetTypeName(v.GetType());
            build.AppendFormat("{0,-12} ({1,2},{2,2}) {3,2}-{4}\n",name,v.Line,v.Column,v.Span.Start,v.Span.End)
            |> ignore

            let attributes = v.TryGetAttributes();
            if attributes <> null then
                build.AppendFormat("{0,-12} ({1,2},{2,2}) {3,2}-{4}\n","attributes",attributes.Line,attributes.Column,attributes.Span.Start,attributes.Span.End)
                |> ignore

        let result = build.ToString().Trim();
        result

    let rec leafInlines (ci:ContainerInline) =
        [
            if ci <> null then
                for o:Inline in ci do
                    match o with
                    | :? ContainerInline as c ->
                        yield! leafInlines c

                    | :? LeafInline as l ->
                        yield l
                    | _ ->
                        failwith <| o.GetType().Name
        ]

    let rec leafBlocks (cb:ContainerBlock) =
        [
            for b:Block in cb do
                match b with
                | :? ContainerBlock as c ->
                    yield! leafBlocks c
                | :? LeafBlock as lf ->
                    yield lf
                | _ -> failwith <| b.GetType().Name

        ]
        //Path.Combine(__SOURCE_DIRECTORY__,"markdownblocks.md")

type MarkdigTest(output: ITestOutputHelper) =
    let text =
        Path.Combine(@"C:\Program Files\Typora\resources\app\Docs","Markdown Reference.md")
        |> File.ReadAllText

    let document =
        let pipeline =
            MarkdownPipelineBuilder()//.UseAdvancedExtensions()
                .UseAbbreviations()
                //.UseAutoIdentifiers() //*导致多个空的[]:
                .UseCitations()
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseEmphasisExtras()
                .UseFigures()
                .UseFooters()
                .UseFootnotes() //*导致正文中的[^xxx]丢失,[^xxx]：评论落到文章底部
                .UseGridTables()
                .UseMathematics()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                .UseDiagrams()
                //.UseAutoLinks()
                .UseGenericAttributes()
                .Build()
        Markdown.Parse(text, pipeline)

    [<Fact>]
    member this.``NormalizeRenderer Test``() =
        use writer = new StringWriter()
        let normalizer = new NormalizeRenderer(writer)
        normalizer.Render(document) |> ignore
        let raw = writer.ToString()
        File.WriteAllText(@"d:\Markdown Reference.md",raw)
        //output.WriteLine(raw)
    

    [<Fact>]
    member this.``markdown syntax Type Test``() =
        let res = Markdigs.markdownType document
        File.WriteAllText(@"d:\markdown syntax Type.txt",res)

        //output.WriteLine(res)

    [<Fact>]
    member this.``get content Test``() =
        for desc:Block in document do
            match desc with
            | :? HeadingBlock as h ->
                let raw =
                    Markdigs.leafInlines h.Inline
                    |> List.map(fun l -> l.ToString())
                    |> String.concat ""
                output.WriteLine(sprintf "%A" raw)

            | :? QuoteBlock as q ->
                for qq in Markdigs.leafBlocks q do
                    let raw =
                        Markdigs.leafInlines qq.Inline
                        |> List.map(fun l -> l.ToString())
                        |> String.concat ""

                    output.WriteLine(sprintf "%A" raw)

            | :? ListBlock as l ->
                for qq in Markdigs.leafBlocks l do
                    let raw =
                        Markdigs.leafInlines qq.Inline
                        |> List.map(fun l -> l.ToString())
                        |> String.concat ""

                    output.WriteLine(sprintf "%A" raw)

            | :? ParagraphBlock as h ->
                let raw =
                    Markdigs.leafInlines h.Inline
                    |> List.map(fun l -> if l = null then "" else l.ToString())
                    |> String.concat ""
                output.WriteLine(sprintf "%A" raw)

            | :? FencedCodeBlock as h ->
                let content =
                    [
                        //match h.Lines with
                        //| null -> ()
                        //| _ ->
                            for l in h.Lines.Lines do
                                yield sprintf "%A" <| l.ToString()
                    ]
                //output.WriteLine(sprintf "%s,%s" h.Info h.Arguments)

                //let raw =
                //    Markdigs.leafInlines h.Inline
                    //|> List.map(fun l -> if l = null then "" else l.ToString())
                    //|> String.concat ""
                for l in h.Lines.Lines do
                    output.WriteLine(sprintf "%A" l)

            | _ ->  output.WriteLine(desc.GetType().Name)

