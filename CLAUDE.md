# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Heretic.InteractiveFiction is a .NET framework (NuGet package) for building text adventure games in the classic 1980s style. It provides the parser, game loop, object model, event system, and printing subsystem; game authors implement their specific game world on top of the framework.

- **Main library:** `src/Heretic.InteractiveFiction/` — targets `netstandard2.1`, C# 11
- **Unit tests:** `test/Heretic.InteractiveFiction.Tests/` — targets `net7.0`, xUnit + FluentAssertions
- **Sample game:** `test/Heretic.InteractiveFiction.TestGame/` — full reference implementation

## Commands

```bash
# Build everything
dotnet build Heretic.InteractiveFiction.Solution.sln

# Run all tests
dotnet test test/Heretic.InteractiveFiction.Tests/Heretic.InteractiveFiction.Tests.csproj

# Run a single test class
dotnet test test/Heretic.InteractiveFiction.Tests/ --filter "ClassName=ArticleHandlerTest"

# Run a single test method
dotnet test test/Heretic.InteractiveFiction.Tests/ --filter "FullyQualifiedName~MethodName"

# Pack the NuGet package
dotnet pack src/Heretic.InteractiveFiction/Heretic.InteractiveFiction.csproj -c Release
```

## Architecture

### Object Hierarchy

All game entities derive from `AHereticObject` (split across 3 partial files):
- `AHereticObject.cs` — core properties (key, name, breakable, pickable, drinkable, etc.)
- `AHereticObject.Descriptions.cs` — description handling
- `AHereticObject.EventHandler.cs` — 40+ events raised before/after every action

Concrete types: `Player`, `Character`, `Item`, `Location`.

### Universe (Central State)

`Universe` is the single source of truth: holds `ActiveLocation`, `ActivePlayer`, `LocationMap` (navigation graph), all items/characters, quest state, and coordinates periodic events. Everything that needs game state receives a `Universe` reference through constructor injection.

### Command Pattern

Every verb maps to a sealed record implementing `ICommand`:
```csharp
internal sealed record BreakCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ...) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent) { ... }
}
```

- `VerbKey` enum — identifies each verb
- `AdventureEvent` — carries the parsed predicate and up to two resolved objects (`ObjectOne`, `ObjectTwo`)
- `CommandExecutor` — registry (`Dictionary<VerbKey, ICommand>`) built at startup; all 57 commands wired here
- Commands return `bool` indicating whether the action produced output

### Input Pipeline

`GameLoop` → `InputProcessor` → `InputAnalyzer` → `CommandExecutor`

`InputAnalyzer` tokenizes input, resolves pronouns (via `PronounHandler`), handles prepositions, and performs object lookup through `ObjectHandler`. It returns an `AdventureEvent`. The grammar layer (`IGrammar` / `GermanGrammar`) is consulted for language-specific parsing.

### Event System

Objects raise events around every interaction. Pattern: `BeforeBreak` → break logic → `AfterBreak`. Game authors hook these events to script game-specific behavior without subclassing. Event args are specialized types in `GamePlay/EventSystem/EventArgs/`.

### Exception-Based Flow Control

Control flow for game-ending conditions uses exceptions:
- `GameWonException` — player won
- `QuitGameException` — player quit
- One exception type per verb in `Heretic.InteractiveFiction.Exceptions/` (49 types total), used to signal mid-command interruptions

`GameLoop` catches these at the top level.

### Localization

Resources are embedded `.resx` files. The neutral/default language is German. Strings include pipe-delimited alternative names (`Lampe|Laterne|Licht`). Implement `IResourceProvider` to supply custom resource sets. `ArticleHandler` and `AdjectiveDeclinationHandler` handle grammatical declension for German.

### Printing Subsystem

`IPrintingSubsystem` abstracts all output. `BaseConsolePrintingSubsystem` is the built-in console implementation. Game authors can substitute any output mechanism (GUI, network, etc.) by implementing this interface.

## Key Conventions

- Commands are `internal sealed record` — never public classes
- `AHereticObject` partial files must stay in their three-file split (core / descriptions / event handlers)
- New verbs require: a `VerbKey` entry, a command record, an exception type, resource strings for the verb, and registration in `CommandExecutor.InitCommands()`
- `ObjectHandler` is the only place that resolves game objects from parsed input strings — don't duplicate that logic in commands
