using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record ReadCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);

                if (item.IsReadable)
                {
                    try
                    {
                        var readItemEventArgs = new ReadItemEventArgs()
                        {
                            OptionalErrorMessage = adventureEvent.Predicate != default
                                ? adventureEvent.Predicate.ErrorMessage
                                : string.Empty
                        };

                        item.OnBeforeRead(readItemEventArgs);

                        var result = string.IsNullOrWhiteSpace(item.LetterContentDescription)
                            ? PrintingSubsystem.Resource(BaseDescriptions.NO_LETTER_CONTENT)
                            : PrintingSubsystem.FormattedResource(BaseDescriptions.LETTER_CONTENT,
                                item.LetterContentDescription);

                        item.OnRead(readItemEventArgs);

                        item.OnAfterRead(readItemEventArgs);

                        return result;
                    }
                    catch (ReadException ex)
                    {
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }

                return PrintingSubsystem.Resource(BaseDescriptions.NOTHING_TO_READ);
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_READ);
    }
}