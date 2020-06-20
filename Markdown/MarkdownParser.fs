module Markdown.MarkdownParser

open System.Text.RegularExpressions

let (|Heading|_|) line =
    let re = Regex @"^(#{1,6})(\s+(?<c>.+?))?(\s+#+)?\s*$"

    let mat = re.Match(line)
    if mat.Success then
        Some(mat.Groups.[1].Length, mat.Groups.["c"].Value)
    else
        None

let (|Blank|_|) line =
    let re = Regex @"^\s*$"

    let mat = re.Match(line)
    if mat.Success then
        Some()
    else
        None

let (|Blockquote|_|) line =
    let re = Regex @"^\>\s*(.*)$"

    let mat = re.Match(line)
    if mat.Success then
        Some(mat.Groups.[1].Value)
    else
        None

let (|ListItem|_|) line =
    let pTask = @"^- \[(.*?)\]"
    let pOrder = @"^(\d{1,9})[.)]"
    let pBullet = @"^[-*]"

    let pat = sprintf "((?<task>%s)|(?<order>%s)|(?<bullet>%s) +)(?<content>.*)\s*$" pTask pOrder pBullet
    let re = Regex pat

    let mat = re.Match(line)
    if mat.Success then
        Some(mat.Groups.[1].Value, mat.Groups.["content"].Value)
    else
        None

let (|CodeFence|_|) line =
    let re = Regex @"^(`{3,})(?<lang>[^\r\n`]*)$"

    let mat = re.Match(line)
    if mat.Success then
        let len = mat.Groups.[1].Value
        Some(len, mat.Groups.["lang"].Value)
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
    let re = Regex @"^\[(.*?)\]:\s*(\S+?)(\s+""(?<tip>.*?)"")?\s*$"

    let mat = re.Match(line)
    if mat.Success then
        let g = mat.Groups
        Some(g.[1].Value, g.[2].Value, g.["tip"].Value)
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

let getBlocks lines =
    let indentline line =
        let re = Regex("^\s*")
        let m = re.Match(line)
        m.Value,m.Result("$'")

    let rec loop blocks = function
        | [] -> blocks |> List.rev
        | line :: restLines ->
            let indent, line = indentline line
            match line with
            | Heading (level,txt) ->
                let blocks = (indent,Heading(level,txt))::blocks
                loop blocks restLines

            | ThematicBreak _ ->
                let blocks = (indent,ThematicBreak) ::blocks
                loop blocks restLines

            | Blank _ ->
                let blocks = (indent,Blank) ::blocks
                loop blocks restLines

            | Blockquote x ->
                let blocks = (indent,Blockquote(x)) ::blocks
                loop blocks restLines

            | ListItem(b,s) ->
                let blocks = (indent,ListItem(b,s)) ::blocks
                loop blocks restLines

            //| Tasklist(b,s) ->
            //    let blocks = (indent,TaskList(b,s)) ::blocks
            //    loop blocks restLines

            //| Olist(i,s) ->
            //    let blocks = (indent,OrderedList(i,s)) :: blocks
            //    loop blocks restLines

            //| Ulist s ->
            //    let blocks = (indent,BulletList(s)) :: blocks
            //    loop blocks restLines

            | TOC _ ->
                let blocks = (indent,TOC) :: blocks
                loop blocks restLines

            | Footnote s ->
                let blocks = (indent,Footnote(s)) :: blocks
                loop blocks restLines

            | LinkReferenceDefinition (id,a,t) ->
                let blocks = (indent,LinkReferenceDefinition(id,a,t)) :: blocks
                loop blocks restLines

            | CodeFence (start,lang) -> // multiline block
                let ii =
                    restLines
                    |> List.tryFindIndex(fun ln -> Regex.IsMatch(ln,sprintf @"^\s*`{%d,}\s*$" start.Length))

                match ii with
                | Some i ->
                    let content = restLines |> List.take i |> String.concat "\r\n"
                    let close = restLines.Item i
                    let restLines =  restLines |> List.skip (i+1)
                    let blocks = (indent,FencedCode(start, lang, content, close)) :: blocks
                    loop blocks restLines
                | _ ->
                    let blocks = (indent, Paragraph(line)) :: blocks
                    loop blocks restLines

            | MathOpen _ -> // multiline block
                let ii =
                    restLines
                    |> List.tryFindIndex(function MathOpen _ -> true | _ -> false)

                match ii with
                | Some i ->
                    let content = restLines |> List.take i |> String.concat "\n"
                    let restLines =  restLines |> List.skip (i+1)
                    let blocks = (indent,Math content) :: blocks
                    loop blocks restLines
                | _ ->
                    let blocks = (indent,Paragraph(line)) :: blocks
                    loop blocks restLines

            | _ ->
                let blocks = (indent,Paragraph(line)) :: blocks
                loop blocks restLines

    loop [] lines |> List.toArray
