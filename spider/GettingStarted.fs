module spider.GettingStarted

open System.Text
open System.Net.Http

///獲取unicode網頁文本
let getDocumentAsync(url:string) =
    task {
        let client = new HttpClient()
        return! client.GetStringAsync url
    }

let GB18030 =
    //https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding.registerprovider?redirectedfrom=MSDN&view=netcore-3.1
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
    Encoding.GetEncoding("GB18030")

let convertToGB18030(bytes:byte[]) = GB18030.GetString(bytes)

