module InsightClub.Db.Migrator.Program

open DbUp
open System
open System.IO
open System.Reflection
open System.Collections.Generic


[<EntryPoint>]
let main _ =
  let config = "config"

  let connectionString = File.ReadAllText config

  let comparer =
    { new IComparer<string> with
        member _.Compare(s1, s2) =
          let n1 = Int32.Parse s1.[ 1 .. s1.IndexOf '.' - 1 ]
          let n2 = Int32.Parse s2.[ 1 .. s2.IndexOf '.' - 1 ]

          n1 - n2 }

  let upgrader =
    DeployChanges
      .To
      .PostgresqlDatabase(connectionString)
      .WithTransactionPerScript()
      .WithScriptsEmbeddedInAssembly(
        Assembly.GetExecutingAssembly(),
        fun p -> p.EndsWith ".psql")
      .WithScriptNameComparer(comparer)
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
