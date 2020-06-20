namespace Markdown

open System.Text.RegularExpressions

//解析单行
type MarkdownLine =
    | Heading of int * string
    | Blockquote of string
    | TaskList of bool * string
    | BulletList of string
    | OrderedList of int * string
    | FencedCode of string * string
    | Math of string
    | ThematicBreak
    | Blank
    | TOC
    | Footnote of string * string
    | LinkReferenceDefinition of string * string * string
    | Paragraph of string
    //| TableRow of string[]
    //| TableDelimiter of string[]

module MarkdownLineParser =

    let (|Heading|_|) line =
        let re = Regex @"^(#{1,6})\s+(.*)$"

        let mat = re.Match(line)
        if mat.Success then
            Some(mat.Groups.[1].Length,mat.Groups.[2].Value)
        else
            None

    let (|Blank|_|) line =
        let re = Regex @"^(\s*)$"

        let mat = re.Match(line)
        if mat.Success then
            Some(mat.Value)
        else
            None

    let (|Blockquote|_|) line =
        let re = Regex @"^\>\s+(.*)$"

        let mat = re.Match(line)
        if mat.Success then
            Some(mat.Groups.[1].Value)
        else
            None

    let (|Tasklist|_|) line =
        let re = Regex @"^- \[(.*?)\]\s+(.*)$"

        let mat = re.Match(line)
        if mat.Success then
            Some(mat.Groups.[1].Value.Trim() <> "", mat.Groups.[2].Value)
        else
            None

    let (|Olist|_|)line =
        let re = Regex @"^(\d+)\.\s+(.*)$"

        let mat = re.Match(line)

        if mat.Success then
            Some(int mat.Groups.[1].Value, mat.Groups.[2].Value)
        else
            None

    let (|Ulist|_|) line =
        let re = Regex @"^[-*]\s+(.*)$"

        let mat = re.Match(line)

        if mat.Success then
            Some(mat.Groups.[1].Value)
        else
            None

    let (|CodeFence|_|) line =
        let re = Regex @"^```\s*(?<lang>.*)$"

        let mat = re.Match(line)
        if mat.Success then
            Some(mat.Groups.["lang"].Value)
        else
            None

    let (|MathOpen|_|) line =
        let re = Regex @"^\$\$\s*$"

        let mat = re.Match(line)
        if mat.Success then
            Some()
        else
            None

    let (|Footnote|_|) line =
        let re = Regex @"^\[\^(.*?)\]:\s+(.*)$"

        let mat = re.Match(line)
        if mat.Success then
            Some(mat.Groups.[1].Value,mat.Groups.[2].Value)
        else
            None

    let (|ThematicBreak|_|) line =
        let re = Regex @"^(-{3,}|\*{3,})\s*$"

        let mat = re.Match(line)
        if mat.Success then
            Some()
        else
            None

    let (|TOC|_|) line =
        let re = Regex(@"^\[TOC\]\s*$",RegexOptions.IgnoreCase)

        let mat = re.Match(line)
        if mat.Success then
            Some()
        else
            None

    let (|LinkReferenceDefinition|_|) line =
        let re = Regex @"^\[(.*?)\]:\s+(.*?)(\s+""(?<tip>.*?)"")?\s*$"

        let mat = re.Match(line)
        if mat.Success then
            let g = mat.Groups
            Some(g.[1].Value,g.[2].Value,g.["tip"].Value)
        else
            None

    let (|TableDelimiter|_|) line =
        let re = Regex(@"^\s*\|?\s*((?<a>:?-+:?)\s*\|)*\s*(?<b>:?-+:?)\s*\|?\s*$", RegexOptions.IgnoreCase)

        let mat = re.Match(line)
        if mat.Success then
            let a = mat.Groups.["a"]
            let b = mat.Groups.["b"]

            let cols =
                [
                    for c in a.Captures -> c.Value
                    yield b.Value
                ]|> Array.ofList

            Some(cols)
        else
            None

    let ParseDoc lines =
        let rec loop blocks = function
            | [] -> blocks |> List.rev
            | line :: restLines ->
                match line with
                | Heading (level,txt) ->
                    let blocks = Heading(level,txt)::blocks
                    loop blocks restLines

                | ThematicBreak _ ->
                    let blocks = ThematicBreak ::blocks
                    loop blocks restLines

                | Blank _ ->
                    let blocks = Blank ::blocks
                    loop blocks restLines

                | Blockquote x ->
                    let blocks = Blockquote(x) ::blocks
                    loop blocks restLines

                | Tasklist(b,s) ->
                    let blocks = TaskList(b,s) ::blocks
                    loop blocks restLines

                | Olist(i,s) ->
                    let blocks = OrderedList(i,s) :: blocks
                    loop blocks restLines

                | Ulist s ->
                    let blocks = BulletList(s) :: blocks
                    loop blocks restLines

                | TOC _ ->
                    let blocks = TOC :: blocks
                    loop blocks restLines

                | Footnote s ->
                    let blocks = Footnote(s) :: blocks
                    loop blocks restLines

                | LinkReferenceDefinition (id,a,t) ->
                    let blocks = LinkReferenceDefinition(id,a,t) :: blocks
                    loop blocks restLines
                    
                | CodeFence lang ->
                    let ii =
                        restLines
                        |> List.tryFindIndex(function CodeFence _ -> true | _ -> false)

                    match ii with
                    | Some i ->
                        let content = restLines |> List.take i |> String.concat "\n"
                        let restLines =  restLines |> List.skip (i+1)
                        let blocks = FencedCode(lang, content) :: blocks
                        loop blocks restLines
                    | _ ->
                        let blocks = Paragraph(line) :: blocks
                        loop blocks restLines

                | MathOpen _ ->
                    let ii =
                        restLines
                        |> List.tryFindIndex(function MathOpen _ -> true | _ -> false)

                    match ii with
                    | Some i ->
                        let content = restLines |> List.take i |> String.concat "\n"
                        let restLines =  restLines |> List.skip (i+1)
                        let blocks = Math content :: blocks
                        loop blocks restLines
                    | _ ->
                        let blocks = Paragraph(line) :: blocks
                        loop blocks restLines
                        
                | _ ->
                    let blocks = Paragraph(line) :: blocks
                    loop blocks restLines
        loop lines