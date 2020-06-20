module printfiles.bitwise

open System
open Markdown
open System.IO
open System.Text.RegularExpressions

let norm lines =
    let mapper (block:MarkdownBlock) =
        match block with
        | Heading (level,content) -> Heading(level, tidy.norm content)
        | Blockquote _
        | ListItem _
        | Footnote _
        | LinkReferenceDefinition _
        | Paragraph _ ->
            block.content
            |> tidy.norm
            |> tidy.backstick
            |> block.setContent
        | FencedCode(start,lang, content,close) ->
            let lang = if String.IsNullOrWhiteSpace(lang) then "FSharp" else lang
            FencedCode(start,lang, tidy.vb2fs content,close)

        //| Math (content)
        //| ThematicBreak
        //| Blank
        //| TOC
        | _ -> block

    MarkdownRender.smartReplace (fun(i,b)-> i, mapper b) lines

let re = Regex(@"\.md$", RegexOptions.IgnoreCase)

let fsfile(filename) =
    let folder = @"D:\repos\xp44mm\Notebook\F#"
    let sourcePath = Path.Combine(folder,filename)
    let targetPath = Path.Combine(folder,re.Replace(filename,"(fs)$0"))

    let lines = File.ReadAllLines(sourcePath)
    let text = lines |> norm |> String.concat "\r\n"

    File.Delete targetPath
    File.WriteAllText(targetPath,text)

let main () = 
    let files = 
        [
            "Advanced Techniques.md"
        ]
    for f in files do
        fsfile f
