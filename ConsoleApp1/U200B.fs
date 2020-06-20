module U200B

open System
open Markdown
open System.IO
open System.Text.RegularExpressions

let re = Regex(@"\.md$", RegexOptions.IgnoreCase)

let fsfile() =
    //let folder = @"D:\repos\xp44mm\markdig\src\Markdig.Tests\Specs"
    //let filename = "CommonMark.md"

    let folder = @"C:\Program Files\Typora\resources\app\Docs"
    let filename = "Markdown Reference.md"


    let sourcePath = Path.Combine(folder,filename)
    //let targetPath = Path.Combine(folder,re.Replace(filename,"(fs)$0"))

    let text = File.ReadAllText(sourcePath)

    let count = Regex.Matches(text,@"\u200B").Count
    Console.WriteLine("count:{0}",count)
    //File.Delete targetPath
    //File.WriteAllText(targetPath,text)

let main () = 
    fsfile()
