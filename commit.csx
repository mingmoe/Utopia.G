#!/usr/bin/env dotnet-script
#r "nuget: Spectre.Console, 0.48.0"
#nullable enable

using System.Diagnostics;
using Spectre.Console;

AnsiConsole.MarkupLine("This script gui you to generate a Conventional Commit");

var type = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .PageSize(10)
        .Title("What's the type([blue]fix,feat,docs,test,chore[/])?")
        .AddChoices(new[]
        {
            "fix","feat","docs","test","chore"
        }));

Console.WriteLine();
var breakChange = AnsiConsole.Confirm("Is it a [red]BREAK CHANGE[/]?");

var description = AnsiConsole.Prompt(
    new TextPrompt<string>("[blue]Description[/]:"));

var detail = AnsiConsole.Prompt(
    new TextPrompt<string>("[grey][[Optional]][/][blue]Detail[/]:")
    .AllowEmpty());

var output = string.Format("{0}{1}:{2}\n\n{3}",type, breakChange ? "!" : string.Empty,description,detail is null ? string.Empty : detail).Trim();

Console.WriteLine("Result:");
Console.WriteLine("----BEGIN----");
Console.WriteLine($"{output}");
Console.WriteLine("-----END-----");

var commit = AnsiConsole.Confirm("[blue]Make c commit?[/]?");

if(commit){
    try
            {
                using (Process git = new Process())
                {
                    git.StartInfo.UseShellExecute = false;
                    git.StartInfo.FileName = "git";
                    git.StartInfo.CreateNoWindow = true;
                    git.StartInfo.ArgumentList.Add("commit");
                    git.StartInfo.ArgumentList.Add("-m");
                    git.StartInfo.ArgumentList.Add(output);
                    git.Start();
                    git.WaitForExit();
                    Environment.Exit(git.ExitCode);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
}
