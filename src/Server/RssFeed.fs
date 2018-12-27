module RssFeed

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open FSharp.Data

open Shared

// type Rss = XmlProvider<Schema="./atom.xsd">
type Rss = XmlProvider<"http://stackoverflow.com/feeds/tag/f%23">

let fetchResults (url : string) : Task<Message list> = (
    task {
        let! data = Rss.AsyncLoad url
        return data.Entries
            |> Array.map (fun entry ->
                {
                    Id = entry.Id
                    Timestamp = entry.Updated.DateTime
                    Source = data.Title.Value
                    Title = entry.Title.Value
                    Link = Some entry.Link.Href
                    Author = Some entry.Author.Name
                })
            |> Array.toList
    }
)
