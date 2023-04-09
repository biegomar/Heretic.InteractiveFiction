using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.TestGame.GamePlay;

IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler();
var gameLoop = new GameLoop(gamePrerequisitesAssembler);

gameLoop.Run();