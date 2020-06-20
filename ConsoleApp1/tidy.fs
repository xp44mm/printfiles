module printfiles.tidy
open System

open System.Text.RegularExpressions

let vb2fs text =
    let text = Regex.Replace(text,@"(?<=\W)_\r?\n\s*","") //vb续行
    let text = Regex.Replace(text,@"^([^']*)' *(.*\S)", "$1// $2",RegexOptions.Multiline)//注释：每行的第一个单引号
    let text = Regex.Replace(text,@"\[(Enum|Delegate|Assembly|Module|Object)\]","$1")
    let text = Regex.Replace(text,@"(\w+)\s*&=","$1 <- $1 +")
    let text = Regex.Replace(text,@"(\w+)\s*\+=","$1 <- $1 +")
    let text = Regex.Replace(text,@"(\w+)\s*-=","$1 <- $1 -")
    let text = Regex.Replace(text,"&","+")

    let text = Regex.Replace(text,@"(?<!\.)\bGetType\(([^()]+)\)","typeof<$1>")
    let text = Regex.Replace(text,@"(?<!\.)\bGetType\(","typeof<")
    let text = Regex.Replace(text,@"(?<=\w)\(Of ([^()]+)\)","<$1>")
    let text = Regex.Replace(text,@"(?<=\w)\(Of ","<")

    let text = Regex.Replace(text,@"^(\s*)Dim (\w+)\(\) As (\w+) = \{([^}]*)\}","$1let $2: $3[] = [|$4|]",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"New Object\(\)\s*\{([^}]*)\}","[|$1|]",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^Imports\b","open", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Inherits (.*\S)","$1inherit $2()",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)(?:Private |Public )?Function (.+) As (.*\S)","$1member this.$2 : $3 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Private |Public )?Sub (.*\S)","$1member this.$2 =",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"\bOptional(?= By(Val|Ref))","?")
    let text = Regex.Replace(text,@"\b(ByVal ParamArray)\b","[<ParamArray>] ByVal")

    let text = Regex.Replace(text,@"\bByVal (\w+)\(\) As (\w+)","$1 : $2[]")
    let text = Regex.Replace(text,@"\bByVal (\w+) As ","$1 : ")

    let text = Regex.Replace(text,@"^(\s*)(?:Dim|Const) (\w+) As New ","$1let $2 = new ",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Dim|Const) (\w+)\(\) As (\w+)","$1let $2: $3[]",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Dim|Const) (\w+) As ","$1let $2: ",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)Using (\w+) As New ","$1use $2 = new ",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)Select Case (.*\S)","$1match $2 with",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Case (.+):","$1| $2 ->",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(End \w+|Next|Loop)\b(?<follow>.*)$","$1${follow}",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Return ","$1",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Catch (\w+) As (\w+)","$1with | :? $3 as $2 -> ",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)For Each (\w+) As (.+) In (.*\S)","$1for $2: $3 in $4 do", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)For (\w+) As .+? = (.+?) To (.*\S)","$1for $2 = $3 to $4 do", RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)Do While (.*\S)","$1while $2 do",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Do Until (.*\S)","$1while not <| $2 do",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)If (.+?) Then\b","$1if $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)ElseIf (.+) Then\b","$1elif $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Else\b","$1else",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"(?<!//.*)\b(type)\b","myType")//变量名与关键字冲突

    let text = Regex.Replace(text,@"\bDirectCast\((.*), (.+)\)","$1 :?> $2")
    let text = Regex.Replace(text,@"\bCDbl\b","float ")
    let text = Regex.Replace(text,@"\bTypeOf (\w+) Is (\w+)\b","$1.GetType() = typeof<$2>")

    let text = Regex.Replace(text,@"(?<!// .*)\bAnd\b","&&&")
    let text = Regex.Replace(text,@"(?<!// .*)\bOr\b","|||")
    let text = Regex.Replace(text,@"(?<!// .*)\bXor\b","^^^")
    let text = Regex.Replace(text,@"(?<!// .*)\bAndAlso\b","&&")
    let text = Regex.Replace(text,@"(?<!// .*)\bOrElse\b","||")
    let text = Regex.Replace(text,@"(?<!// .*)\bMod\b","%")

    let text = Regex.Replace(text,@"(?<!// .*)\bNot\b","not <|")
    let text = Regex.Replace(text,@"(?<!// .*)\b(Nothing)\b","null")
    let text = Regex.Replace(text,@"(?<!// .*)\b(IsNot)\b","<>")
    let text = Regex.Replace(text,@"(?<!// .*)\b(Is)\b","=")
    let text = Regex.Replace(text,@"(?<!// .*)\b(Integer)\b","Int32")
    let text = Regex.Replace(text,@"(?<!// .*)\b(Date)\b","DateTime")
    let text = Regex.Replace(text,@"(?<!// .*)\bThrow New ","raise <| new ")
    let text = Regex.Replace(text,@"(?<!// .*)\b(Try|False|True|New)\b",fun m1 -> m1.Value.ToLower())

    let text = Regex.Replace(text,"(?<!\")\"\"\"\"(?!\")","\"\\\"\"")
    let text = Regex.Replace(text,"\"(.)\"c"+ @"\b","'$1'")
    let text = Regex.Replace(text,"\"([^\"]|\"\")*\"(?!\")",MatchEvaluator(fun mat ->
        if mat.Value.[1..mat.Value.Length-2].Split([|'\\';'"'|]).Length > 1 then
            "@" + mat.Value
        else
            mat.Value
    ))//vb字符串->f#逐字字符串

    let maybe = sprintf "(%s)?\s*"

    let reTypes =
        let pat =
            [
                "^(\s*)"
                maybe "Public|Friend|Private|Protected|Protected Friend"
                maybe "Shadows|NotInheritable|MustInherit"
                "(?<type>(Class|Module|Interface|Enum|Structure))\s+(?<name>.*\S)"
            ] |> String.concat ""

        new Regex(pat, RegexOptions.IgnoreCase ||| RegexOptions.Multiline)

    let text = reTypes.Replace(text,(fun mat ->
        match mat.Groups.["type"].Value with
        | "Class" -> mat.Result("$1type ${name}() =")
        | "Module" -> mat.Result("$1module ${name} =")
        | _ -> mat.Value
    ))

    let text = Regex.Replace(text,@"\b(T)\b", "'T")

    text

let norm text =
    let text = Regex.Replace(text,@"(?<!!)\[(.*?)\]\(.*?\)","$1")//删除超链接
    let text = Regex.Replace(text,"(?<=\S) {2,}(?=\S)"," ")//删除单词间的连续空格
    let text = Regex.Replace(text,"\*(Xx+)\*","$1")//删除*Xxxx*两边的星号

    text

let backstick text =
    let dot = @"(?=\w*[a-zA-Z])\w+(\.(?=\w*[a-zA-Z])\w+)+"//匹配带点号的单词
    let camel = @"(?=\w*[a-z])(?=\w*[A-Z])\w+"//匹配名称大小驼峰

    let pat = sprintf @"\b((?<dot>%s)|(?<camel>%s))\b" dot camel
    let re = Regex pat
    let skips =
            set [
                "If";
                "Figure";
                "Table";
                "I";
                "Chapter";
                "Microsoft";"Windows";
                "KB";
                "Unicode";
                "Server";
                "Visual";"Studio";"XP";"NT";"CD";"ROM";"DVD";
                "C";"Web";
                "UNC";"Notepad";
                "TCP";"IP";
                "ASP";"NET";"Framework";
                "Version";"VB";
                "Basic";
                "XML";
                "ACL";"ACLs"
                "ACE";"ACEs";"DACL";"SACL"
                "LAN";
                "Internet"
                "URL"
                "API"
                "MSDN"
                "SID"
                "SDDL"
                ]

    let text = re.Replace(text,MatchEvaluator(fun mat ->
        let up = mat.Result("$`")
        let follow = mat.Result("$'")

        if mat.Groups.["dot"].Success then
            if Regex.IsMatch(mat.Value,"\.jpg$") then
                mat.Value
            else
                sprintf "`%s`" mat.Value
        elif mat.Groups.["camel"].Success then

            if skips.Contains mat.Value then //专有名称
                mat.Value
            elif Regex.IsMatch(mat.Value,"\w[A-Z]") then //非首字母有大写的
                sprintf "`%s`" mat.Value
            elif String.IsNullOrWhiteSpace(up) || Regex.IsMatch(up, @"[\S-[\w,]] *$") then //仅首字母大写的单词，位于句首(前面有标点符号)
                mat.Value
            else
                sprintf "`%s`" mat.Value
        else mat.Value
    
    
    ))
    text

    //let text = Regex.Replace(text, @"(?<!`)\b(?=.*[a-zA-Z])(\w+(\.\w+)+)\b(?!`)", MatchEvaluator(fun mat ->
    //    if Regex.IsMatch(mat.Value,"\.jpg$") then
    //        mat.Value
    //    else
    //        sprintf "`%s`" mat.Value
    //))//匹配带点号的单词

    //let text = //匹配名称大小驼峰
    //    let skips =
    //        set [
    //            //"The";
    //            //"If";
    //            //"When";
    //            //"This";
    //            //"Without";
    //            //"Another";
    //            //"For";
    //            //"You";"It";"Me";
    //            //"There";"Here";"Before";"Once";
    //            //"Use";
    //            //"Finally";"In";"On";"By";
    //            //"As";"A";"An";"Each";"See";
    //            //"However";"How";
    //            //"Alternatively";"Because";"Remote";
    //            //"Rather";"Not";"Most";"Using";
    //            //"These";"Specific";
    //            //"First";"Second";"After";
    //            //"Other";"Buffering";"Next";"Reading";"Conveniently";
    //            //"Keep";"Creating";

    //            "Figure";
    //            "Table";
    //            "I";
    //            "Chapter";
    //            "Microsoft";"Windows";
    //            "KB";
    //            "Unicode";
    //            "Server";
    //            "Visual";"Studio";"XP";"NT";"CD";"ROM";"DVD";
    //            "C";"Web";
    //            "UNC";"Notepad";
    //            "TCP";"IP";
    //            "ASP";"NET";"Framework";
    //            "Version";"VB";
    //            "Basic";
    //            "XML";
    //            "ACL";"ACLs"
    //            "ACE";"ACEs";"DACL";"SACL"
    //            "LAN";
    //            "Internet"
    //            "URL"
    //            "API"
    //            "MSDN"
    //            "SID"
    //            "SDDL"
    //            ]

    //    Regex.Replace(text, @"(?<![`.]\S*)\b(?=.*[a-z])(?=.*[A-Z])(\w+)\b(?!\S*`)", MatchEvaluator(fun mat ->
    //        let up = mat.Result("$`")
    //        let follow = mat.Result("$'")

    //        if skips.Contains mat.Value then //专有名称
    //            mat.Value
    //        elif Regex.IsMatch(mat.Value,"\w[A-Z]") then //非首字母有大写的
    //            sprintf "`%s`" mat.Value
    //        elif String.IsNullOrWhiteSpace(up) || Regex.IsMatch(up, "(\.\)? |\(|\*\* |: )$") then //仅首字母大写的单词，位于句首
    //            mat.Value
    //        else
    //            sprintf "`%s`" mat.Value
    //    ))
