module printfiles.vb2fs

open System
open System.IO
open ReflectionatRunTime

//let folder = @"D:\repos\xp44mm\Programming Microsoft Visual Basic 2005 The Language\18 Reflection\EventInterceptorDemo"
let folder = @"D:\repos\xp44mm\Programming Microsoft Visual Basic 2005 The Language\18 Reflection\ReflectionDemo"

let filenames = 
    [|
        //"EventInterceptor.vb"
        //"Form1.vb"
        "ActionSequence.vb"
    |]

let fsfile() =
    filenames
    |> Array.iter(fun filename ->
        let sourcePath = Path.Combine(folder,filename)
        let targetPath = Path.ChangeExtension(sourcePath,"fs")

        let text = File.ReadAllText(sourcePath)
        let text = text |> tidy.vb2fs

        File.Delete targetPath
        File.WriteAllText(targetPath,text)
    )
    
    //Console.WriteLine(targetPath)


let main () = fsfile()
