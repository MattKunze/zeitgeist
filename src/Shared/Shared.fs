namespace Shared

open System
open Microsoft.AspNetCore.DataProtection

type Counter = { Value : int }

type Message = {
    Id: string
    Timestamp: DateTime
    Source : string
    Title : string
    Link : string option
    Author : string option
}

type User = {
    Name: string
    Email: string option
    Avatar: Uri option
}

type Repository = {
    Name: string
    Description: string option
    Homepage: Uri
}

type Label = {
    Title: string
    Color: string
}

type Commit = {
    Id: string
    Message: string
    Timestamp: DateTimeOffset
    Url: Uri
    Author: User
}

type PushEvent = {
    CheckoutSha: string
    Author: User
    Commits: Commit list
    TotalCommitCount: int
    Repository: Repository
}

type IssueEvent = {
    Description: string
    State: string
    CreatedAt: DateTimeOffset
    Url: Uri
    Author: User
    Labels: Label list
    Repository: Repository
}

type MergeRequestEvent = {
    Description: string
    SourceBranch: string
    TargetBranch: string
    Status: string
    CreatedAt: DateTimeOffset
    Url: Uri
    Author: User
    Labels: Label list
    Repository: Repository
}

type CommentTargetType = Issue | MergeRequest
type CommentTarget = {
    Type: CommentTargetType
    Id: int
    Url: Uri
}

type CommentEvent = {
    Author: User
    Note: string
    CreatedAt: DateTimeOffset
    Url: Uri
    Target: CommentTarget
    Repository: Repository
}

type SourceControlEvent =
    | PushEvent
    | IssueEvent
    | MergeRequestEvent
    | CommentEvent

type ZeitgeistEvent = {
    Details: SourceControlEvent
    Raw: string
}
