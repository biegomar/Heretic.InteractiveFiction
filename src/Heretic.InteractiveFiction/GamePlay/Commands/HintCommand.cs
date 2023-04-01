using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record HintCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem): ICommand
{
    private bool isHintActive;

    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is {} item)
        {
            if (isHintActive)
            {
                return PrintingSubsystem.Hint(item);
            }
            
            return PrintingSubsystem.Resource(BaseDescriptions.HINT_NOT_ACTIVE);
        }
        
        if (adventureEvent.UnidentifiedSentenceParts.Count == 1)
        {
            if (string.Equals(adventureEvent.UnidentifiedSentenceParts.Single(), BaseDescriptions.ON,
                    StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = true;
                return PrintingSubsystem.Resource(BaseDescriptions.HINT_ON);
            }

            if (string.Equals(adventureEvent.UnidentifiedSentenceParts.Single(), BaseDescriptions.OFF,
                    StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = false;
                return PrintingSubsystem.Resource(BaseDescriptions.HINT_OFF);
            }
        }
        else
        {
            if (adventureEvent.UnidentifiedSentenceParts.Count == 0)
            {
                if (isHintActive)
                {
                    return PrintingSubsystem.Hint(Universe.ActiveLocation);
                }
            
                return PrintingSubsystem.Resource(BaseDescriptions.HINT_NOT_ACTIVE);
            }
        }

        return PrintingSubsystem.Resource(BaseDescriptions.HINT_FOR_WHAT);
    }
}