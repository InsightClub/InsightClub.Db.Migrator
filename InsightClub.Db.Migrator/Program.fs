module Program

open DbUp
open System
open System.IO
open System.Reflection


[<EntryPoint>]
let main argv =
  let config =
    #if DEBUG
    "../../../" +
    #endif
    "config"

  let connectionString = File.ReadAllText config

  let upgrader =
    DeployChanges
      .To
      .PostgresqlDatabase(connectionString)
      .WithScriptsEmbeddedInAssembly(
        Assembly.GetExecutingAssembly(),
        fun p -> p.EndsWith ".psql")
      .LogToConsole()
      .Build()

  let result = upgrader.PerformUpgrade()

  if not result.Successful then
    Console.ForegroundColor <- ConsoleColor.Red
    Console.WriteLine result.Error
    #if DEBUG
    Console.ReadKey()
    |> ignore
    #endif
    -1
  else
    Console.ForegroundColor <- ConsoleColor.Green
    Console.WriteLine "Success!"
    #if DEBUG
    Console.ReadKey()
    |> ignore
    #endif
    0
