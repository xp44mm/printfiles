namespace Tests

open Xunit
open Xunit.Abstractions

open System
open System.IO

type PathTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``valid drive and filename separators``() =
        Assert.Equal(Path.AltDirectorySeparatorChar,'/')
        Assert.Equal(Path.DirectorySeparatorChar,'\\')
        Assert.Equal(Path.PathSeparator,';')
        Assert.Equal(Path.VolumeSeparatorChar,':')

    [<Fact>]
    member this.``can't be used in paths and filenames``() =
        // Note: the actual output from following methods includes unprintable characters.
        Assert.Equal<char[]>(Path.GetInvalidPathChars(),[|
        '|'; '\000'; '\001'; '\002'; '\003'; '\004'; '\005'; '\006'; '\007'; '\b';
        '\009'; '\010'; '\011'; '\012'; '\013'; '\014'; '\015'; '\016'; '\017'; '\018';
        '\019'; '\020'; '\021'; '\022'; '\023'; '\024'; '\025'; '\026'; '\027'; '\028';
        '\029'; '\030'; '\031'|])      // => <>|
        Assert.Equal<char[]>(Path.GetInvalidFileNameChars(),[|
        '"'; '<'; '>'; '|'; '\000'; '\001'; '\002'; '\003'; '\004'; '\005'; '\006';
        '\007'; '\b'; '\009'; '\010'; '\011'; '\012'; '\013'; '\014'; '\015'; '\016';
        '\017'; '\018'; '\019'; '\020'; '\021'; '\022'; '\023'; '\024'; '\025'; '\026';
        '\027'; '\028'; '\029'; '\030'; '\031'; ':'; '*'; '?'; '\\'; '/'|])  // => <>|:*?\/
    [<Fact>]
    member this.``temporary directory and temporary file``() =
        output.WriteLine(Path.GetTempPath())
            // => C:\Documents and Settings\Francesco\Local Settings\Temp
        output.WriteLine(Path.GetTempFileName())
            // => C:\Documents and Settings\Francesco\Local Settings\Temp\tmp1FC7.tmp

    [<Fact>]
    member this.``extract information from a file path``() =
        let file = @"C:\MyApp\Bin\MyApp.exe"
        Assert.Equal(Path.GetDirectoryName(file           ),@"C:\MyApp\Bin")
        Assert.Equal(Path.GetFileName(file                ),@"MyApp.exe")
        Assert.Equal(Path.GetExtension(file               ),@".exe")
        Assert.Equal(Path.GetFileNameWithoutExtension(file),@"MyApp")
        Assert.Equal(Path.GetPathRoot(file                ),@"C:\")
        Assert.Equal(Path.HasExtension(file               ),true)
        Assert.Equal(Path.IsPathRooted(file               ),true)

    [<Fact>]
    member this.``the name of the parent directory``() =
        let d = Environment.SystemDirectory
        let winDir: String = Path.GetDirectoryName(d)

        //output.WriteLine(d)
        //output.WriteLine(winDir)

        Assert.Equal(d,@"C:\WINDOWS\system32")
        Assert.Equal(winDir,@"C:\WINDOWS")

    [<Fact>]
    member this.``expands a relative path to an absolute path``() =
        output.WriteLine(Path.GetFullPath("MyApp.Exe"))
        output.WriteLine(Path.GetFullPath(@"c:\public\..\private"))

    [<Fact>]
    member this.``a filename with a different extension``() =
        Assert.Equal(Path.ChangeExtension("MyApp.Exe", "dat"),"MyApp.dat")  // => 

        Assert.Equal(Path.Combine(@"C:\MyApp", "MyApp.Dat"),@"C:\MyApp\MyApp.Dat")    // => 