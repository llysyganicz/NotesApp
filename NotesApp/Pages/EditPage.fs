module EditPage

open Models
open System
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms

type Model = Note

type Field = Title | Content

type Msg =
    | TextChanged of Field * string
    | Save

let initModel() = { Id = Guid.NewGuid(); Title = ""; Content = "" }
let init() = initModel(), Cmd.none

let update msg model =
    match msg with
    | TextChanged (field, newValue) ->
        let note = match field with
                   | Title -> { model with Title = newValue }
                   | Content -> { model with Content = newValue }
        note, Cmd.none
    | Save -> model, Cmd.none

let view model dispatch =
    View.ContentPage(title = "Edit note",
        toolbarItems = [
            View.ToolbarItem(text = "Save", command = (fun () -> dispatch Save))
        ],
        content = View.StackLayout(
            verticalOptions = LayoutOptions.FillAndExpand,
            children = [
                View.Entry(text = model.Title, placeholder = "Title", textChanged = (fun args -> dispatch (TextChanged(Title, args.NewTextValue))))
                View.Label(text = "Content:")
                View.Editor(text = model.Content, textChanged = (fun args -> dispatch (TextChanged(Content, args.NewTextValue))),
                            verticalOptions = LayoutOptions.FillAndExpand)
            ]))