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

let mutable private history : Message list = []

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
    getf "/api/push/%s/%s" (fun req next ctx ->
        let source, title = req

        let msg = {
            Id = Guid.NewGuid().ToString()
            Timestamp = DateTime.Now
            Source = source
            Title = title
            Link = None
            Author = None
        }
        history <- msg :: history
        json history next ctx
    )
    getf "/api/rss/%s" (fun url next ctx ->
        task {
            let currentIds =
                history |> List.map (fun m -> m.Id)

            let! results = RssFeed.fetchResults url
            history <- results
                |> List.filter (fun t -> not (List.contains t.Id currentIds))
                |> List.append history

            return! json history next ctx
        }
    )
    post "/api/gitlab" (fun next ctx ->
        let logger = ctx.GetLogger "gitlab"
        task {
            let! request = ctx.ReadBodyFromRequestAsync()
            logger.LogWarning ("got: " + request) |> ignore

            return! json "ok" next ctx
        }
    )
}
