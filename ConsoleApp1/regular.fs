module regular

open System
open System.IO
open System.Text.RegularExpressions

//let rgx = Regex(@"\\[abtrvfneswdpgzk]")
//let inp = @"\w"
//rgx.IsMatch(inp)

let quoteCode text =
    let text = Regex.Replace(text, @"\b(Nothing)\b", "`null`")
    let text = Regex.Replace(text, @"\b(Friend)\b", "`internal`")
    let text = Regex.Replace(text, @"\b(Shared)\b", "`static`")
    let text = Regex.Replace(text, @"(?<!`)\b(False|True)\b(?!`)",fun mat -> "`" + mat.Value.ToLower() + "`")
    let text = Regex.Replace(text, @"(?<!`)\b(([A-Z]\w*(\.[A-Z]\w*)+))\b(?!`)", "`$1`")//匹配全名称大驼峰
    let text = Regex.Replace(text, @"(?<!`)\b\w+([A-Z]([a-z0-9_]+))+\b(?!`)", "`$0`")//匹配名称大小驼峰

    let branches =
        [
            "Regex"
            "String"
            "Match(es)?"
            "Value"
            "Index"
            "Length"
            "Replace"
            "Compiled"
            "Options"
            "Split"
            "Parse"
            "Escape"
            "Unescape"
            "Groups?"
            "Result"
            "Success"
            "Captures?"

        ] |> String.concat "|"

    let text = Regex.Replace(text, sprintf @"(?<!`)\b(%s)\b(?!`)" branches, "`$1`")

    //识别正则表达式
    let text = Regex.Replace(text, sprintf @"(?<!`)(\S+)(?!`)", MatchEvaluator(fun m ->
            let s = m.Value
            if Regex.IsMatch(s,@"\\\w|\(\?|[.)\]][*+?{]|<.*>|\[\^|0\-9|a\-z",RegexOptions.IgnoreCase) then
                sprintf "`%s`" s
            else
                s
        ))

    //基于上下文的替换
    let text = Regex.Replace(text,@" \^ and \$ "," `^` and `$$` ") //

    let text = Regex.Replace(text,@"`U\.S`\.","U.S.") //将误杀改回去
    let text = Regex.Replace(text,@"`ASP\.NET`","ASP.NET") //将误杀改回去

    let text = Regex.Replace(text,@"^(#.*\S)",(fun g1 -> g1.Value.Replace("`","")),RegexOptions.Multiline) //标题行不加引号

    text

let highlight text =
    let text = Regex.Replace(text,@"^```\s*\r?\n([\w\W]+?)\r?\n```","```F#\n$1\n```",RegexOptions.Multiline)//添加代码块名称
    let text = Regex.Replace(text,@"\[(.*?)\]\(.*?\)","$1")//删除超链接
    let text = Regex.Replace(text," +$","",RegexOptions.Multiline)//删除行尾空白
    let text = Regex.Replace(text,"(?<=\S) {2,}"," ")//删除单词后的连续空格

    text

let getfs text =
    let text = Regex.Replace(text,@"(?<=\W)_\r?\n\s*"," ") //vb续行
    let text = Regex.Replace(text,@"\[Enum\]","Enum")
    let text = Regex.Replace(text,@"(\w+)\s*&=","$1 <- $1 +")
    let text = Regex.Replace(text,@"(\w+)\s*\+=","$1 <- $1 +")
    let text = Regex.Replace(text,@"(\w+)\s*-=","$1 <- $1 -")

    let text = Regex.Replace(text,"' =>","// =>")
    let text = Regex.Replace(text,@"^(\s*)' ", "$1// ", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^Imports\b","open", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Module (.*\S)","$1module $2 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Private |Public )?Function (.+) As (.*\S)","$1let $2 : $3 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Private |Public )?Sub (.*\S)","$1let $2 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Select Case (.*\S)","$1match $2 with",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Case (.+):","$1| $2 ->",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^\s*((End \w+)|Next|Loop)\s*$","",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Return ","$1",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Dim|Const) (\w+) As New ","$1let $2 = new ",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Using (\w+) As New ","$1use $2 = new ",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)(?:Dim|Const) (\w+)\(\) As (\w+)","$1let $2: $3[]",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Dim|Const) (\w+) As ","$1let $2: ",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Catch (\w+) As Exception","$1with | $2 -> ",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"\bByVal (.+?)\(\) As (\w+)","$1 : $2[]")
    let text = Regex.Replace(text,@"\bByVal (.+?) As ","$1 : ")
    let text = Regex.Replace(text,@"\bThrow New ","raise <| new ")

    let text = Regex.Replace(text,"&","+")

    let text = Regex.Replace(text,@"^(\s*)For Each (\w+) As (.+) In (.*\S)","$1for $2: $3 in $4 do", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Do While (.*\S)","$1while $2 do",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Do Until (.*\S)","$1while not <| $2 do",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)If (.+?) Then\b","$1if $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)ElseIf (.+) Then\b","$1elif $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Else\b","$1else",RegexOptions.Multiline)

    //let text = Regex.Replace(text,@"(?<!\.)\bGetType\(([^()]+)\)","typeof<$1>")
    //let text = Regex.Replace(text,@"(?<!\.)\bGetType\(","typeof<")
    let text = Regex.Replace(text,@"(?<=\w)\(Of ([^()]+)\)","<$1>")
    let text = Regex.Replace(text,@"(?<=\w)\(Of ","<")
    let text = Regex.Replace(text,@"\bTypeOf (\w+) Is\b","$1.GetType() =")

    let text = Regex.Replace(text,@"\bAndAlso\b","&&")
    let text = Regex.Replace(text,@"\bOrElse\b","||")
    let text = Regex.Replace(text,@"\bAnd\b","&&&")
    let text = Regex.Replace(text,@"\bOr\b","|||")
    //let text = Regex.Replace(text,@"\bNot\b","not <|")
    let text = Regex.Replace(text,@"\b(Nothing)\b","null")
    let text = Regex.Replace(text,@"\b(IsNot)\b","<>")
    let text = Regex.Replace(text,@"\b(Is)\b","=")
    let text = Regex.Replace(text,@"\b(False|True)\b",fun m1 -> m1.Value.ToLower())
    let text = Regex.Replace(text,@"\b(Integer)\b","int")
    //let text = Regex.Replace(text,@"\(\s*\)\s*: Type = \{(.*)\}",": Type[] = [|$1|]")

    //let text = Regex.Replace(text,"(?<!\")\"\"\"\"(?!\")","\"\\\"\"")
    let text = Regex.Replace(text,"\"(.)\"c"+ @"\b","'$1'")

    text

let folder = @"D:\编程书籍\F#"

let readFile filename =
    File.ReadAllText(Path.Combine(folder,filename))

let writeFile (filename:string) appendix text =
    let tgt = filename.Replace(".md",appendix + ".md")
    let path = (Path.Combine(folder,tgt))
    File.Delete path
    File.WriteAllText(path, text)

let filenames =
    [
        //"## Regular Expression Overview"
        //"## Regular Expression Types"
        "## Regular Expressions at Work"
    ] |> List.map(fun s ->s + ".md")

let writeHL() =
    for filename in filenames do
            readFile filename
            |> highlight
            |> writeFile filename ""

let writeFs() =
    for filename in filenames do
            readFile filename
            |> getfs
            |> writeFile filename "(fs)"

let writeMain() =
    for filename in filenames do
        readFile filename
        |> quoteCode
        |> writeFile filename "(main)"

let main ()=
    //writeHL()
    //writeFs()
    writeMain()
