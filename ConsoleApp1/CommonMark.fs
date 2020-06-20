module printfiles.CommonMark

open System
open Markdown
open System.IO
open System.Text.RegularExpressions

let replaceOutCodeBlock (text:string) =
    let text = text.Replace("](@)","](#)")
    let text = Regex.Replace(text, @"\b(\d+)-{2,}(\d+)\b","$1-$2")
    let text = Regex.Replace(text, @"(?<=\S)-{2,}(?=\S)","—")
    text

let hasHtml (text:string) = Regex.IsMatch(text, @"^\.\r?\n<", RegexOptions.Multiline)

let replaceInCodeBlock (text:string) =
    if Regex.IsMatch(text, @"^ *`{3,}", RegexOptions.Multiline) then
        let msep = Regex.Match(text, @"(?<!\r)\r?\n\.\r?\n")
        let md,html =
            if msep.Success then
                msep.Result("$`"), msep.Result("$'")
            else text,""
        let md = Regex.Replace(md,"^(.*)","|$1",RegexOptions.Multiline)
        let html =
            if Regex.IsMatch(html, @"^ *`{3,}", RegexOptions.Multiline) then
                Regex.Replace(html,"^(.*)","|$1",RegexOptions.Multiline)
            else html
        md+msep.Value+html
    else text

let norm lines =
    let mapper (block:MarkdownBlock) =
        match block with
        | Heading _ 
        | Blockquote _
        | ListItem _
        | Footnote _
        | LinkReferenceDefinition _
        | Paragraph _ ->
            block.content
            |> replaceOutCodeBlock
            |> block.setContent
        | FencedCode(start,lang, content,close) ->
            let lang = if hasHtml content then "html" else lang
            let content = replaceInCodeBlock content
            FencedCode(start, lang, content, close)

        //| Math (content)
        //| ThematicBreak
        //| Blank
        //| TOC
        | _ -> block

    MarkdownRender.smartReplace (fun(i,b)-> i, mapper b) lines

let re = Regex(@"\.md$", RegexOptions.IgnoreCase)

let main () =
    let folder = @"D:\repos\xp44mm\markdig\src\Markdig.Tests\Specs"
    let filename = "CommonMark.md"

    let sourcePath = Path.Combine(folder,filename)
    let targetPath = Path.Combine(folder,re.Replace(filename,"(fs)$0"))

    let lines = File.ReadAllLines(sourcePath)
    let lines = lines |> norm

    File.Delete targetPath
    File.WriteAllLines(targetPath,lines)

