namespace Markdown
open System

module Render =
    let normalize = function
        | Heading (l, content) -> sprintf "%s %s\n" (String('#',l)) content
        | Blockquote content -> sprintf "> %s\n" content
        | TaskList (comp,content) -> 
            sprintf "- [%c] %s\n" (if comp then 'x' else ' ') content
        | OrderedList (n,content) ->
            sprintf "%d. %s\n" n content
        | BulletList content ->
            sprintf "* %s\n" content

        | FencedCode (lang,content) ->
            sprintf "```%s\n%s\n```\n" lang content

        | Math (content) ->
            sprintf "$$\n%s\n$$\n" content

        | Footnote(id,note) -> 
            sprintf "[^%s]: %s\n" id note

        | ThematicBreak ->
            "---"
        | Blank -> "\n"

        | TOC -> "[TOC]"

        | LinkReferenceDefinition (id,a,tip) ->
            if tip = "" then
                sprintf "[%s]: %s\n" id a
            else
                sprintf "[%s]: %s \"%s\"\n" id a tip

        | Paragraph content -> sprintf "%s\n" content
        //| TableRow of string[]
        //| TableAlign of string[]
        

