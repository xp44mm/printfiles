module printfiles.ActionSequence //将vb源代码文件转换为fs源代码文件


open System.IO
open System.Text.RegularExpressions


let vb2fs text =
    let text = Regex.Replace(text,@"(?<=\W)_\r?\n\s*"," ") //vb续行
    let text = Regex.Replace(text,@"^(.*)' (.*)", "$1// $2")//注释
    let text = Regex.Replace(text,@"\[(Enum|Delegate|Assembly)\]","$1")
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
    let text = Regex.Replace(text,@"^(\s*)(?:Private |Public )?Function (.+) As (.*\S)","$1member this.$2 : $3 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)(?:Private |Public )?Sub (.*\S)","$1member this.$2 =",RegexOptions.Multiline)

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
    let text = Regex.Replace(text,@"^(\s*)For (\w+) As (\w+) = 0 To (.*\S)","$1for $2: $3 in [0..$4] do", RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)Do While (.*\S)","$1while $2 do",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Do Until (.*\S)","$1while not <| $2 do",RegexOptions.Multiline)

    let text = Regex.Replace(text,@"^(\s*)If (.+?) Then\b","$1if $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)ElseIf (.+) Then\b","$1elif $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Else\b","$1else",RegexOptions.Multiline)

    //let text = Regex.Replace(text,@"(?<!\.)\bGetType\(([^()]+)\)","typeof<$1>")
    //let text = Regex.Replace(text,@"(?<!\.)\bGetType\(","typeof<")

    let text = Regex.Replace(text,@"(?<!//.*)\b(type)\b","myType")//变量名错误


    let text = Regex.Replace(text,@"\bDirectCast\((.*), (.+)\)","$1 :?> $2")
    let text = Regex.Replace(text,@"\bCDbl\b","float ")
    let text = Regex.Replace(text,@"\bTypeOf (\w+) Is (\w+)\b","$1.GetType() = typeof<$2>")

    let text = Regex.Replace(text,@"\bAndAlso\b","&&")
    let text = Regex.Replace(text,@"\bOrElse\b","||")
    let text = Regex.Replace(text,@"\bAnd\b","&&&")
    let text = Regex.Replace(text,@"\bOr\b","|||")
    let text = Regex.Replace(text,@"(?<!//.*)\bNot\b","not <|")
    let text = Regex.Replace(text,@"\b(Nothing)\b","null")
    let text = Regex.Replace(text,@"\b(IsNot)\b","<>")
    let text = Regex.Replace(text,@"\b(Is)\b","=")
    let text = Regex.Replace(text,@"\b(Try|False|True)\b",fun m1 -> m1.Value.ToLower())
    let text = Regex.Replace(text,@"\b(Integer)\b","Int32")
    let text = Regex.Replace(text,@"\bThrow New ","raise <| new ")

    let text = Regex.Replace(text,"(?<!\")\"\"\"\"(?!\")","\"\\\"\"")
    let text = Regex.Replace(text,"\"(.)\"c"+ @"\b","'$1'")
    let text = Regex.Replace(text,"\"([^\"]|\"\")*\"(?!\")","@$0")//vb字符串是f#逐字字符串

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
    

let fsfile() =
    let folder = @"D:\Application Data\Visual Studio\Projects\SchemaReaderTest\SchemaReaderTest"
    let filename = "ActionSequence"
    let sourcePath = Path.Combine(folder,filename + ".vb")
    let targetPath = Path.Combine(folder,filename + ".fs")

    let vb = File.ReadAllText(sourcePath)

    let fs = vb2fs vb
    
    File.Delete targetPath
    File.WriteAllText(targetPath,fs)

let main () = fsfile()
