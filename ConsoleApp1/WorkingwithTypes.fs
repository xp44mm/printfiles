module WorkingwithTypes

open System
open System.IO
open System.Text.RegularExpressions

let folder = @"C:\Users\cuishengli\Documents"
let filename = "Working with Types.md"

let readFile () =
    File.ReadAllText(Path.Combine(folder,filename))

let quoteCode text =
    let text = Regex.Replace(text, @"(?<!`)\b(([A-Z]\w*(\.[A-Z]\w*)+))\b(?!`.)", "`$1`")

    let text = Regex.Replace(text, @"(?<!`)\b(Object|Form|\w*Exception|\w*Type|String|TypeResolve|AssemblyResolve|Name|FullName|Assembly|Module)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(Is[A-Z]\w+)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(Get[A-Z]\w+)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(Can[A-Z]\w+)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(\w+Info)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(\.ctor)\b(?!`)", "`$1`")

    //BindingFlags.Public
    let text = Regex.Replace(text, @"\b(The Public and NonPublic)\b", "The `Public` and `NonPublic`")

    let text = Regex.Replace(text, @"(?<!`)\b(Format|Concat|BindingFlags)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(NonPublic|Instance|Static|DeclaredOnly|FlattenHierarchy)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(FindMembers|MethodBase)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(Member|Position|DefaultValue|Invoke|MethodBody|TryOffset|Flags|ExceptionHandlingClause)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(GenericParameterAttributes)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(Serializable|NonSerialized|DllImport|StructLayout|FieldOffset)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(CustomAttributeData|CustomAttributeTypedArgument|CustomAttributeNamedArgument)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(Constructor|ConstructorArguments|NamedArguments)\b(?!`)", "`$1`")
    let text = Regex.Replace(text, @"(?<!`)\b(FormatTypedArgument|Obsolete)\b(?!`)", "`$1`")

    let text = Regex.Replace(text, @"\bList\(Of `String`\)", "`List<String>`")
    let text = Regex.Replace(text, @"\bDictionary\(Of `String`, Integer\)", "`Dictionary<String, Int32>`")

    //MustInherit NotInheritable ReadOnly MustOverride NotOverridable AddHandler RemoveHandler
    let text = Regex.Replace(text, @"\b(Nothing)\b", "`null`")
    let text = Regex.Replace(text, @"\b(Friend)\b", "`internal`")
    let text = Regex.Replace(text, @"\b(Shared)\b", "`static`")

    let text = Regex.Replace(text,@"(?<!`)\b(False|True|Public|Private|Try|Finally)\b(?!`)",fun m1 -> "`" + m1.Value.ToLower() + "`")
    let text = Regex.Replace(text,@"^(#.*\S)",(fun g1 -> g1.Value.Replace("`","")),RegexOptions.Multiline)

    text

let highlight text =
    let text = Regex.Replace(text,@"^```\s*\r?\n(.+?)\r?\n```","```F#\n$1\n```",RegexOptions.Multiline|||RegexOptions.Singleline)
    text

let getfs text =
    let text = Regex.Replace(text,@"(?<=\W)_\r?\n\s*"," ")
    let text = Regex.Replace(text,@"\[Enum\]","Enum")
    let text = Regex.Replace(text,@"(\w+)\s*&=","$1 <- $1 +")
    let text = Regex.Replace(text,"&","+")
    let text = Regex.Replace(text,@"(?<!\.)\b(Public |Private )","")
    let text = Regex.Replace(text,"' =>","// =>")
    let text = Regex.Replace(text,@"^(\s*)' ", "$1// ", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^Imports\b","open", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Module (.*\S)","$1module $2 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Function (.+) As (.*\S)","$1let $2 : $3 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Sub (.*\S)","$1let $2 =",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Select Case (.*\S)","$1match $2 with",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Case (.*\S)","$1| $2 ->",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^\s*((End \w+)|Next)\s*$","",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Return ","$1",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Dim (.+) As ","$1let $2: ",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)For Each (.*) As (.*) In (.*\S)","$1for $2: $3 in $4 do", RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)If (.+?) Then\b","$1if $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)ElseIf (.+) Then\b","$1elif $2 then",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"^(\s*)Else\b","$1else",RegexOptions.Multiline)
    let text = Regex.Replace(text,@"\bByVal (.+?) As ","$1 : ")
    let text = Regex.Replace(text,@"(?<!\.)\bGetType\(([^()]+)\)","typeof<$1>")
    let text = Regex.Replace(text,@"(?<!\.)\bGetType\(","typeof<")
    let text = Regex.Replace(text,@"(?<=\w)\(Of ([^()]+)\)","<$1>")
    let text = Regex.Replace(text,@"(?<=\w)\(Of ","<")
    let text = Regex.Replace(text,@"\bTypeOf (\w+) Is\b","$1.GetType() =")

    let text = Regex.Replace(text,@"\bAndAlso\b","&&")
    let text = Regex.Replace(text,@"\bOrElse\b","||")
    let text = Regex.Replace(text,@"\bAnd\b","&&&")
    let text = Regex.Replace(text,@"\bOr\b","|||")
    let text = Regex.Replace(text,@"\bNot\b","not <|")
    let text = Regex.Replace(text,@"\b(Nothing)\b","null")
    let text = Regex.Replace(text,@"\b(IsNot)\b","<>")
    let text = Regex.Replace(text,@"\b(Is)\b","=")
    let text = Regex.Replace(text,@"\b(Try|False|True)\b",fun m1 -> m1.Value.ToLower())
    let text = Regex.Replace(text,@"\b(Integer)\b","int")
    let text = Regex.Replace(text,@"\(\s*\)\s*: Type = \{(.*)\}",": Type[] = [|$1|]")

    let text = Regex.Replace(text,"(?<!\")\"\"\"\"(?!\")","\"\\\"\"")
    let text = Regex.Replace(text,"\"(.)\"c"+ @"\b","'$1'")

    text

let writeFile appendix text =
    let tgt = filename.Replace(".md",appendix + ".md")
    let path = (Path.Combine(folder,tgt))
    File.Delete path
    File.WriteAllText(path, text)

let writeFs() =
    let text =
        readFile()
        |> getfs
    writeFile "(fs)" text

let writeMain() =
    let text =
        readFile()
        |> quoteCode
    writeFile "(main)" text

let main ()=
    writeFs()
    //writeMain()
