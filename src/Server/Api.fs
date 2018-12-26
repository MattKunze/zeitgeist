module Api

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open Giraffe
open Saturn

open Shared

let getInitCounter() : Task<Counter> = task { return { Value = 42 } }

let webApp = router {
    get "/api/init" (fun next ctx ->
        task {
            let! counter = getInitCounter()
            return! Successful.OK counter next ctx
        })
}
