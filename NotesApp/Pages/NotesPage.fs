module NotesPage
open System
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms
open Models

type Model = { Notes: Note list }

type Msg =
    | AddNote
    | DeleteNote of Guid
    | NoteSelected of Guid

let init () = { Notes = [] }, Cmd.none

let update msg model =
    match msg with
    | AddNote -> model, Cmd.none
    | DeleteNote noteId -> 
        let notes = model.Notes |> List.filter (fun note -> note.Id <> noteId)
        { model with Notes = notes }, Cmd.none
    | NoteSelected noteId -> model, Cmd.none

let view model dispatch =
    View.ContentPage(
        title = "Notes",
        toolbarItems = [
            View.ToolbarItem(
                text = "Add note",
                command = (fun () -> dispatch AddNote)
            )
        ],
        content = View.ListView(
            horizontalOptions = LayoutOptions.FillAndExpand,
            verticalOptions = LayoutOptions.FillAndExpand,
            items = [
                for note in model.Notes do
                    yield View.StackLayout(
                        horizontalOptions = LayoutOptions.FillAndExpand,
                        orientation = StackOrientation.Horizontal,
                        children = [
                            View.StackLayout(
                                horizontalOptions = LayoutOptions.FillAndExpand,
                                orientation = StackOrientation.Vertical,
                                children = [
                                    View.Label(text = note.Title, fontAttributes = FontAttributes.Bold)
                                    View.Label(text = note.Content)
                                ]
                            )
                            View.Button(text = "-", command = (fun _ -> dispatch (DeleteNote note.Id)))
                        ],
                        gestureRecognizers = [
                            View.TapGestureRecognizer(command = (fun _ -> dispatch (NoteSelected note.Id)))
                        ]
                    )
            ]
        )
    )