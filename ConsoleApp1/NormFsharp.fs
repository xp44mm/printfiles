module NormFsharp

open System
open System.IO
open System.Text.RegularExpressions
open Markdown

open printfiles

let normFs lines =
    let norm text =
        let text = Regex.Replace(text,"(?<=\S) {2,}(?=\S)"," ")//删除单词间的连续空格
        text

    let backstick text =
        let text = Regex.Replace(text, @"(?<!`)\b(\w+(\.\w+)+)\b(?!`)", "`$1`")//匹配带点号的单词
        let text = Regex.Replace(text, @"(?<!`)\b\w+([A-Z]([a-z0-9_]+))+\b(?!`)", "`$0`")//匹配名称大小驼峰
        text

    let mapper (block:MarkdownBlock) =
        match block with
        | Heading (l, content) -> Heading (l, norm content)
        
        | Blockquote _
        | ListItem _
        //| TaskList _
        //| OrderedList _
        //| BulletList _
        | Footnote _
        | LinkReferenceDefinition _
        | Paragraph _ ->
            block.setContent(block.content|>norm|>backstick)
        //| FencedCode(lang, content) ->
        //    let lang = if String.IsNullOrWhiteSpace(lang) then "VB" else lang.Trim()
        //    FencedCode (lang, content)

        | _ -> block

    MarkdownRender.smartReplace (fun(i,b)-> i, mapper b) lines
    

let io =
    let folder = @"D:\repos\xp44mm\Notebook\F#"
    let filename = "Applied Object-Oriented Programming.md"
    FileVersion(folder,filename)
    
let fsfile() =
    let lines = File.ReadAllLines(io.targetPath())
    let text = normFs lines |> String.concat "\n"
    io.writeFile("norm", text)

let main () =
    fsfile()
