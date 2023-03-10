using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record TalkCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne != default)
        {
            if (adventureEvent.ObjectOne is Character character)
            {
                if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(character))
                {
                    try
                    {
                        var containerObjectEventArgs = new ContainerObjectEventArgs()
                            { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                        character.OnBeforeTalk(containerObjectEventArgs);

                        var result = PrintingSubsystem.Talk(character);
                        character.OnTalk(containerObjectEventArgs);

                        character.OnAfterTalk(containerObjectEventArgs);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }

                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            return PrintingSubsystem.Resource(BaseDescriptions.DONT_TALK_TO_ITEM);
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_TALK);
    }
}