// See https://aka.ms/new-console-template for more information

using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.LogCabin;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

IResourceProvider resourceProvider = new ResourceProvider();
IPrintingSubsystem printingSubsystem = new ConsolePrinting();
IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler();

var universe = new Universe(printingSubsystem, resourceProvider);
var gameLoop = new GameLoop(printingSubsystem, universe, gamePrerequisitesAssembler);

gameLoop.Run();