using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record HintCommand(IPrintingSubsystem PrintingSubsystem): ICommand
{
    private bool isHintActive;

    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.UnidentifiedSentenceParts.Count == 1)
        {
            if (string.Equals(adventureEvent.UnidentifiedSentenceParts.Single(), BaseDescriptions.ON,
                    StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = true;
            }

            if (string.Equals(adventureEvent.UnidentifiedSentenceParts.Single(), BaseDescriptions.OFF,
                    StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = false;
            }
        }

        return isHintActive
            ? PrintingSubsystem.Resource(BaseDescriptions.HINT_ON)
            : PrintingSubsystem.Resource(BaseDescriptions.HINT_OFF);
    }
}