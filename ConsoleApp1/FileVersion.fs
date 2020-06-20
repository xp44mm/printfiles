namespace printfiles

open System.IO
open System.Text.RegularExpressions


type FileVersion(folder,filename) =
    static let re = Regex(@"(\(.*\))?\.md$", RegexOptions.IgnoreCase)

    ///为文件名称添加副名称，如x.ext -> x(v).ext
    member this.targetPath(?version) =
        let version = defaultArg version ""
        let filename =
            if version <> "" then
                re.Replace(filename, sprintf "(%s).md" version)
            else
                filename
        Path.Combine(folder,filename)

    member this.writeFile(version, text) =
        let path = this.targetPath(version)
        //let path = Path.Combine(folder,tgt)
        File.Delete path
        File.WriteAllText(path, text)
