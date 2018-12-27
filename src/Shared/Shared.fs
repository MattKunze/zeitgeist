namespace Shared

open System

type Counter = { Value : int }

type Message = {
    Id: string
    Timestamp: DateTime
    Source : string
    Title : string
    Link : string option
    Author : string option
}
