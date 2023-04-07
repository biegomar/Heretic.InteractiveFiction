using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record HelpCommand(IHelpSubsystem HelpSubsystem, IVerbHandler VerbHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.UnidentifiedSentenceParts.Any())
        {
            var listOfVerbs = new List<Verb>();
            foreach (var part in adventureEvent.UnidentifiedSentenceParts)
            {
                listOfVerbs.AddRange(VerbHandler.ExtractPossibleVerbs(part));
            }

            var realVerbList = this.VerbHandler.Verbs.Where(v => listOfVerbs.Select(x => x.Key).Contains(v.Key))
                .ToList();
            return HelpSubsystem.Help(realVerbList);
        }
        
        return HelpSubsystem.Help();
    }
}