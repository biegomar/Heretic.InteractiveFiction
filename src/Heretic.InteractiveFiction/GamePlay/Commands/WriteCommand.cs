using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record WriteCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        try
        {
            var writeEventArgs = new WriteEventArgs()
            {
                Text = string.Join(" ", adventureEvent.UnidentifiedSentenceParts),
                OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage
            };
                
            Universe.ActiveLocation.OnWrite(writeEventArgs);
                
            return true;
        }
        catch (WriteException ex)
        {
            return PrintingSubsystem.Resource(ex.Message);
        }
    }
}