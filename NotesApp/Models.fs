module Models

open System

type Note = 
    { Id: Guid
      Title: string
      Content: string }