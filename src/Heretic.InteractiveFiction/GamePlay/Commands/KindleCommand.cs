using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record KindleCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);

                try
                {
                    var containerObjectEventArgs = new KindleItemEventArgs()
                    {
                        OptionalErrorMessage = adventureEvent.Predicate != default
                            ? adventureEvent.Predicate.ErrorMessage
                            : string.Empty,
                        ItemToUse = adventureEvent.ObjectTwo
                    };
                    item.OnKindle(containerObjectEventArgs);

                    return true;
                }
                catch (KindleException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_KINDLE);
    }
}