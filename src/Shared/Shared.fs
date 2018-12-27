namespace Shared

open System

type Counter = { Value : int }

type Message = {
    Source : string
    Message : string
    Timestamp: DateTime
}
