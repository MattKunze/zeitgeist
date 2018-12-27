module Api

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2

open Giraffe
open Saturn

open Shared
open System.ComponentModel.DataAnnotations

let private getInitCounter() : Task<Counter> = task { return { Value = 42 } }

let mutable private history = [
    {
        Source = "baked in"
        Message = "fake message"
        Timestamp = DateTime.Now
    }
]

let getHistory next ctx =
    json history next ctx

let pushHistory (source, message) (next: HttpFunc) (ctx: HttpContext) =
    let logger = ctx.GetLogger "pushHistory"
    logger.LogWarning("push {s} - {m} ({l})", source, message, history.Length)
    |> ignore

    let msg = {
        Source = source
        Message = message
        Timestamp = DateTime.Now
    }
    history <- msg :: history
    json history next ctx

let webApp = router {
    get "/api/init" (fun next ctx ->
        task {
            let! counter = getInitCounter()
            return! json counter next ctx
        })
    get "/api/history" (fun next ctx ->
        task {
            return! json history next ctx
        })
    getf "/api/push/%s/%s" pushHistory
}
