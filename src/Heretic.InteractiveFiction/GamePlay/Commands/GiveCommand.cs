using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record GiveCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        //I can only give things to visible people.
        if (adventureEvent.ObjectOne is Character character &&
            ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(character))
        {
            //...and I can give only items that i own.
            if (adventureEvent.ObjectTwo is Item item &&
                ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);

                try
                {
                    var eventArgs = new ContainerObjectEventArgs()
                        { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                    Universe.ActivePlayer.OnBeforeGive(eventArgs);
                    character.OnBeforeGive(eventArgs);
                    item.OnBeforeGive(eventArgs);

                    character.Items.Add(item);
                    Universe.ActivePlayer.RemoveItem(item);

                    Universe.ActivePlayer.OnGive(eventArgs);
                    character.OnGive(eventArgs);
                    item.OnGive(eventArgs);

                    Universe.ActivePlayer.OnAfterGive(eventArgs);
                    character.OnAfterGive(eventArgs);
                    item.OnAfterGive(eventArgs);

                    return true;
                }
                catch (GiveException ex)
                {
                    Universe.ActivePlayer.Items.Add(item);
                    character.RemoveItem(item);
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotOwned();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
    }
}