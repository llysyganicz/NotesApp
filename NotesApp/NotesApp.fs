// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace NotesApp

open System.Diagnostics
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms

module App = 
    type Page = Notes | Edit
    type Model = 
      { NotesModel: NotesPage.Model 
        PageStack: Page list}

    type Msg = 
        | NotesMsg of NotesPage.Msg 
        | PagePopped

    let init () = 
        let notesModel, notesCmd = NotesPage.init()
        { NotesModel = notesModel; PageStack = [Notes] }, Cmd.batch [ Cmd.map NotesMsg notesCmd ]

    let update msg model =
        match msg with
        | NotesMsg msg -> 
            let nextModel, nextCmd = NotesPage.update msg model.NotesModel
            { model with NotesModel = nextModel }, Cmd.map NotesMsg nextCmd
        | PagePopped -> 
            match model.PageStack with
            | h::t -> { model with PageStack = t }, Cmd.none
            | [] -> { model with PageStack = [Notes] }, Cmd.none

    let view (model: Model) dispatch =
        View.NavigationPage(
            pages = [
                for page in model.PageStack |> List.rev do
                    match page with
                    | Notes -> yield NotesPage.view model.NotesModel (NotesMsg >> dispatch)
                    | Edit -> yield View.ContentPage()
            ]
          )

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/tools.html for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/models.html for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


