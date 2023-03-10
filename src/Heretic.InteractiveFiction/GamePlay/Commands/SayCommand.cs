using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record SayCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        //I can only speak to visible people
        if (adventureEvent.ObjectOne is Character character &&
            ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(character))
        {
            var phrase = string.Join(" ", adventureEvent.UnidentifiedSentenceParts);
            var key = ObjectHandler.GetConversationAnswerKeyByName(phrase);
            if (string.IsNullOrEmpty(key))
            {
                return PrintingSubsystem.NoAnswer(phrase);
            }

            try
            {
                var conversationEventArgs = new ConversationEventArgs
                    { Phrase = key, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                character.OnSay(conversationEventArgs);
            }
            catch (SayException ex)
            {
                PrintingSubsystem.Resource(ex.Message);
            }

            return true;
        }

        return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
    }
}