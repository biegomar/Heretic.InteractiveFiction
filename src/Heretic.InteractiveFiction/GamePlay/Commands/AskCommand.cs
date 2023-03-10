using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record AskCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler): ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        //I can only speak to visible people in the active location
        if (adventureEvent.ObjectOne is Character character 
            && ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(character))
        {
            //but I can speak about every unhidden or virtual item or character in the world
            var item = adventureEvent.ObjectTwo;
            if (item == default)
            {
                return PrintingSubsystem.NoAnswerToInvisibleObject(character);
            }
                
            try
            {
                var conversationEventArgs = new ConversationEventArgs()
                    { Item = item, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                character.OnAsk(conversationEventArgs);
            }
            catch (AskException ex)
            {
                PrintingSubsystem.NoAnswerToQuestion(ex.Message);
            }

            return true;
        }

        return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
    }
}