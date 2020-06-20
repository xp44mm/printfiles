module gentags

open System.IO

open Newtonsoft.Json
open Newtonsoft.Json.Linq

//从JObject获取成员
//同JavaScript中的Object.entries(root)
let entries (jobj:JObject) =
    seq {
        for e in jobj do
            let prop = e :?> JProperty
            yield (prop.Name, prop.Value)
    }

let getdata () =
    use reader = File.OpenText(Path.Combine(__SOURCE_DIRECTORY__,"tagname.json"))
    let o = JToken.ReadFrom(new JsonTextReader(reader)):?>JObject
    o

let getarray (o:JObject) =
    entries o
    |> Seq.filter(fun(_,jtoken)-> jtoken.Type = JTokenType.Array)
    |> Seq.map(fun(_,jtoken)->jtoken:?>JArray)
    |> Seq.map(fun arr -> [ for e in arr -> string e ] )
    |> Seq.concat
    |> Seq.distinct
    |> Seq.sort
    |> Seq.toArray

let tagnames() = getdata() |> getarray

let template () =
    File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__,"div.js"))

//标识符w变为首字母大写
let capital (w:string) =
    let first = w.[0..0].ToUpper()
    let rest = w.[1..]
    sprintf "%s%s" first rest


let genExports () =
    tagnames()
    |> Array.map(fun tagname -> sprintf "export { %s } from './%s'" tagname tagname)
    |> String.concat "\n"

let genImplements () =
    let template = template ()

    tagnames()
    |> Array.map(fun tagname -> capital tagname, tagname)
    |> Array.map(fun (cn,tn) ->
        let file = sprintf "%s.js" tn
        let text = template.Replace("Div",cn).Replace("div",tn)
        (file,text)
    )


let main () =
    let dir = @"c:/tags/"

    //删除目录下的所有文件
    Directory.GetFiles(dir)
    |> Array.iter(fun f -> File.Delete(f))

    let ex = genExports()
    File.WriteAllText(dir + "index.js", ex)

    for (file,text) in genImplements() do
        File.WriteAllText(dir + file, text)

