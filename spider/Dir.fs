module spider.Dir

open System.IO

let solutionPath = DirectoryInfo(__SOURCE_DIRECTORY__).Parent.FullName
let xp44mm = DirectoryInfo(solutionPath).Parent.FullName
let hanchuancaolu = Path.Combine(xp44mm, "hanchuancaolu")
let ecma262 = Path.Combine(xp44mm, "ecma262")