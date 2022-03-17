module U200B

open System
open System.IO
open System.Text.RegularExpressions

let re = Regex(@"\.md$", RegexOptions.IgnoreCase)

let fsfile() =

    let folder = @"C:\Program Files\Typora\resources\app\Docs"
    let filename = "Markdown Reference.md"


    let sourcePath = Path.Combine(folder,filename)

    let text = File.ReadAllText(sourcePath)

    let count = Regex.Matches(text,@"\u200B").Count
    Console.WriteLine("count:{0}",count)

let main () = 
    fsfile()
