module Client

open Elmish
open Elmish.React
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
open Fulma
open Thoth.Json

open Shared

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = {
    Counter: Counter option
    History: list<Message> option
}

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| Increment
| Decrement
| InitialCountLoaded of Result<Counter, exn>
| InitialHistoryLoaded of Result<list<Message>, exn>

let initialCounter = fetchAs<Counter> "/api/init" (Decode.Auto.generateDecoder())
let initialHistory = fetchAs<list<Message>> "/api/history" (Decode.Auto.generateDecoder())

let loadData task msg =
    Cmd.ofPromise
        task
        []
        (Ok >> msg)
        (Error >> msg)

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { Counter = None; History = None }
    let loadCountCmd = loadData initialCounter InitialCountLoaded
    let loadHistoryCmd = loadData initialHistory InitialHistoryLoaded
    initialModel, Cmd.batch [ loadCountCmd; loadHistoryCmd ]

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel.Counter, msg with
    | Some counter, Increment ->
        let nextModel = { currentModel with Counter = Some { Value = counter.Value + 1 } }
        nextModel, Cmd.none
    | Some counter, Decrement ->
        let nextModel = { currentModel with Counter = Some { Value = counter.Value - 1 } }
        nextModel, Cmd.none
    | _, InitialCountLoaded (Ok initialCount) ->
        let nextModel = { currentModel with Counter = Some initialCount }
        nextModel, Cmd.none
    | _, InitialHistoryLoaded (Ok initialHistory) ->
        let nextModel = { currentModel with History = Some initialHistory }
        nextModel, Cmd.none
    | _ -> currentModel, Cmd.none

let centerAlignment =
    Content.Modifiers [
        Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]

let navbar title =
    Navbar.navbar [ Navbar.Color IsPrimary ]
        [ Navbar.Item.div []
            [ Heading.h2 []
                [ str title ] ] ]

let showCounter = function
| { Counter = Some counter } -> string counter.Value
| { Counter = None } -> "Loading..."

let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ]
        [ str txt ]

let counter model dispatch =
    Container.container [] [
        Content.content [ centerAlignment ]
            [ Heading.h3 [] [
                str ("Press buttons to manipulate counter: " + showCounter model) ] ]
        Columns.columns [] [
            Column.column [] [ button "-" (fun _ -> dispatch Decrement) ]
            Column.column [] [ button "+" (fun _ -> dispatch Increment) ] ] ]

let cardMargin = Props [ Style [ CSSProp.Margin "1em 0" ] ]
let messageCard (message : Shared.Message) =
    Card.card [ cardMargin ] [
        Card.header [] [
            Card.Header.title []
                [ str message.Source ] ]
        Card.content [] [
            Content.content [] [
                str message.Message ] ] ]

let history model dispatch =
    Container.container [] (
        match model with
        | { History = Some history } -> List.map messageCard history
        | { History = None } -> [ str "Loading..." ]
    )

let safeFooter =
    let components =
        span []
           [
             a [ Href "https://saturnframework.github.io" ] [ str "Saturn" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io/elmish/" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://mangelmaxime.github.io/Fulma" ] [ str "Fulma" ]
           ]

    Footer.footer [] [
        Content.content [ centerAlignment ]
            [ p []
                [ strong [] [ str "SAFE Template" ]
                  str " powered by: "
                  components ] ] ]

let view (model : Model) (dispatch : Msg -> unit) =
    div [] [
        navbar "SAFE Template"
        counter model dispatch
        history model dispatch
        safeFooter ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
