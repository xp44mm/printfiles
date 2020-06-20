namespace Markdown

//解析单行
type MarkdownBlock =
    | Heading of int * string
    | Blockquote of string
    | ListItem of string * string
    | FencedCode of start:string * info:string * content:string * close:string
    | Math of string
    | ThematicBreak
    | Blank
    | TOC
    | Footnote of string * string
    | LinkReferenceDefinition of string * string * string
    | Paragraph of string
    //| PipeTable of string[][]

    member this.content =     
        match this with
        | Heading (l, content) -> content
        | Blockquote(content) -> content
        | ListItem(marker,content) -> content
        | Footnote(id,note) -> note
        | LinkReferenceDefinition (label,link,tip) -> tip
        | Paragraph content -> content
        | FencedCode (start,lang,code,close) -> code
        | Math (content) -> content
        //| ThematicBreak
        //| Blank
        //| TOC
        | _ -> ""

    member this.setContent (newContent) = 
        match this with
        | Heading (l, _) -> Heading (l, newContent)
        | Blockquote(_) -> Blockquote(newContent)
        | ListItem(marker,content) -> ListItem(marker,newContent)
        | Footnote(id,_) -> Footnote(id,newContent)
        | LinkReferenceDefinition (id,link,_) -> LinkReferenceDefinition(id,link,newContent)
        | Paragraph (_) -> Paragraph newContent
        | FencedCode (start,lang,content,close) -> FencedCode(start,lang,newContent,close)
        | Math (_) -> Math (newContent)
        //| ThematicBreak
        //| Blank
        //| TOC
        | _ -> this

    
