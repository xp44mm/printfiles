module hyperscript

open System.IO

let codetemplate ="export function div(props, ...children) { return hyperscript('div', ...Array.from(arguments)) }"

let main () =
    let dir = @"C:\Application Data\Visual Studio\Projects\lix\lix\ClientApp\src\hyperscript\"
    let path = Path.Combine(dir,"tags.js")

    let code () =
        let exports =
            gentags.tagnames()
            |> Array.map(fun tag -> codetemplate.Replace("div",tag))
        [
            "import { hyperscript } from './hyperscript'"
            ""
            yield! exports
        ]
        |> String.concat "\r\n"
    
    if File.Exists(path) then File.Delete(path)
    File.WriteAllText(path, code())


