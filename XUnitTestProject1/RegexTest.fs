namespace Tests

open Xunit
open Xunit.Abstractions

open System.Text.RegularExpressions

type RegexTest(output: ITestOutputHelper) =
    [<Fact>]
    member this.``Match Result``() =
        let re =  Regex @"\b(\w*[A-Z]\w*)\b"
        let input = "Enables or disables one or more regular expression options. For example, it allows case sensitivity to be turned on or off in the middle of a pattern. "
        for m in re.Matches(input) do
            let up = m.Result("$`")
            let follow = m.Result("$'") 
            output.WriteLine(sprintf "%A" <| (up,follow))