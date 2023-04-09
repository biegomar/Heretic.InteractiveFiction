using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public interface IGamePrerequisitesAssembler
{
    public IPrintingSubsystem PrintingSubsystem { get; set; } 
    public IResourceProvider ResourceProvider { get; set; }
    public IHelpSubsystem HelpSubsystem { get; set; }
    public IGrammar Grammar { get; set; } 
    public IVerbHandler VerbHandler { get; set; }
    public ScoreBoard ScoreBoard { get; set; }
    public Universe Universe { get; set; }
    public void AssembleGame();
}
