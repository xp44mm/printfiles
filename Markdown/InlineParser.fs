module Markdown.InlineParser

open System.Text.RegularExpressions

let tryMatch pat inp =
    let re = Regex pat
    let m = re.Match(inp)
    if m.Success then Some(m) else None
    
let (|Matched|_|) = tryMatch

type FirstLevel =
    | CodeSpan of string * string
    | AutoLink of string
    | Textual of string


//第一次解析backstick和autolink
let firstLevel inp =
    match inp with

    | "" -> None

    | @"\" -> Some(Textual @"\","")

    | Matched @"^`+" m ->
        match inp with
        | Matched @"^(`+)(.*?)(?<!`)\1(?!`)" mm ->
            let backsticks = mm.Groups.[1].Value
            let code = mm.Groups.[2].Value            
            Some(CodeSpan(backsticks,code),mm.Result("$'"))
        | _ -> Some(Textual m.Value, m.Result("$'"))

    | Matched @"^<" m ->
        match inp with
        | Matched @"^<(\S+?)>" mm ->
            let x = mm.Groups.[1].Value

            if Regex.IsMatch(x,@"[',<>[\]{}]") then//不能存在url需要转义的字符
                Some(Textual m.Value, m.Result("$'"))
            elif Regex.IsMatch(x,@"[/.:@%#?=&]") then //必须存在url分隔符之一的标志
                Some(AutoLink mm.Value,mm.Result("$'"))
            else
                Some(Textual m.Value, m.Result("$'"))
        | _ -> Some(Textual m.Value, m.Result("$'"))

    | Matched @"\\.|[^\\`<]+" m -> Some(Textual m.Value, m.Result("$'"))

    | _ -> Some(Textual inp, "")

