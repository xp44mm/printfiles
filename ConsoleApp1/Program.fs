module printfiles.Program

open System

[<EntryPoint>]
let main argv =
    hyperscript.main()
    printfn "successfully done!"
    0 // return an integer exit code
