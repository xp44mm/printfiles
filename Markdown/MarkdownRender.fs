module Markdown.MarkdownRender

open System

let normalize (block:MarkdownBlock) =
    match block.setContent(block.content) with
    | Heading (l, content) -> sprintf "%s %s" (String('#',l)) content
    | Blockquote content -> sprintf "> %s" content

    | ListItem (marker,content) ->
        sprintf "%s%s" marker content

    | FencedCode (start,lang,content,close) ->
        sprintf "%s%s\r\n%s\r\n%s" start lang content close

    | Math (content) ->
        sprintf "$$\r\n%s\r\n$$" content

    | Footnote(id,note) ->
        sprintf "[^%s]: %s" id note

    | ThematicBreak -> "---"
    | Blank -> ""
    | TOC -> "[TOC]"

    | LinkReferenceDefinition (label,link,tip) ->
        if tip = "" then
            sprintf "[%s]: %s" label link
        else
            sprintf "[%s]: %s \"%s\"" label link tip

    | Paragraph content -> sprintf "%s" content

let smartReplace mapper lines =
    MarkdownParser.getBlocks (Array.toList lines)
    |> Array.map mapper
    |> Array.map(fun(ident,block)-> ident + normalize block)
