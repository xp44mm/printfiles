module spider.GettingStarted

open AngleSharp;
open AngleSharp.Html.Parser
open System.Text
open System
open AngleSharp.Dom
open System.Text.RegularExpressions
open FSharp.Control.Tasks.V2
open System.Net.Http
open AngleSharp.Browser

let ignoreMetaConfig = Configuration.Default.Without<EncodingMetaHandler>()

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

/// 同步解析
let parse (source:string) =
    // We can think of it like a tab in a standard browser.
    let context = BrowsingContext.New(Configuration.Default)    
    let htmlParser = context.GetService<IHtmlParser>()
    htmlParser.ParseDocument(source)

let blockElements = set [
    "ADDRESS"
    "ARTICLE"
    "ASIDE"
    "BLOCKQUOTE"
    "DD"
    "DETAILS"
    "DIALOG"
    "DIV"
    "DL"
    "DT"
    "FIELDSET"
    "FIGCAPTION"
    "FIGURE"
    "FOOTER"
    "FORM"
    "H1"
    "H2"
    "H3"
    "H4"
    "H5"
    "H6"
    "HEADER"
    "HGROUP"
    "HR"
    "LI"
    "MAIN"
    "NAV"
    "OL"
    "P"
    "PRE"
    "SECTION"
    "TABLE"
    "UL"
]

/// 删除块元素临近的空文本
let textContent (e:IElement) =
    let rec loop (e:IElement) =
        [
            for node in e.ChildNodes do
                if node.NodeType = NodeType.Element && node.NodeName = "HR" || node.NodeName = "BR" then
                    yield "\n"
                elif node.NodeType = NodeType.Element then
                    if blockElements.Contains node.NodeName then
                        yield "\n"

                    let elem = node :?> IElement
                    yield! loop elem

                    if blockElements.Contains node.NodeName then
                        yield "\n"

                elif node.NodeType = NodeType.Text then
                    if String.IsNullOrWhiteSpace node.NodeValue then
                        ()
                    else
                        yield Regex.Replace(node.NodeValue.Trim(),@"\s+"," ")
        ]
    loop e 
    |> String.concat ""

let clearChildren(elem:IElement) =
    while elem.HasChildNodes do
        elem.RemoveChild elem.FirstChild
        |> ignore

let rec removeBr (e:IElement) =
    let res = ResizeArray<ResizeArray<INode>>()
    res.Add(ResizeArray<INode>())
    e.ChildNodes
    |> Seq.iter(fun node ->
        if node.NodeType = NodeType.Element then
            let elem = node :?> IElement
            if elem.NodeName = "BR" then
                res.Add(ResizeArray<INode>())
            else
                removeBr elem
                |> Seq.iter(fun ee ->
                    res.[res.Count-1].Add(ee)
                )
        else
            res.[res.Count-1].Add node
    )

    res
    |> Seq.filter(fun childNodes -> childNodes.Count > 0)
    |> Seq.map(fun childNodes ->
        let wp = e.Clone():?> IElement
        clearChildren wp
        childNodes
        |> Seq.iter(fun node ->
            wp.AppendChild node
            |> ignore
        )
        wp
    )
    |> Seq.readonly

