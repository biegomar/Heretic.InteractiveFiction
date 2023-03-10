using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record ChangeLocationCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler, Directions Direction) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (Universe.LocationMap.ContainsKey(Universe.ActiveLocation))
        {
            if (Universe.ActivePlayer.HasClimbed && Universe.ActivePlayer.ClimbedObject != null)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }

            if (Universe.ActivePlayer.IsSitting && Universe.ActivePlayer.Seat != null)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.ALREADY_SITTING);
            }

            var newMapping = Universe.LocationMap[Universe.ActiveLocation].ToList();
            var newLocationMap = newMapping.Where(i => !i.IsHidden).SingleOrDefault(x => x.Direction == Direction);

            if (newLocationMap != default)
            {
                try
                {
                    var oldMapping = Universe.LocationMap[newLocationMap.Location].ToList();
                    var oldLocationMap = oldMapping.SingleOrDefault(i => i.Location == Universe.ActiveLocation);

                    var leaveLocationEventArgs = new LeaveLocationEventArgs(newLocationMap)
                        { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                    var enterLocationEventArgs = new EnterLocationEventArgs(oldLocationMap)
                        { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                    Universe.ActiveLocation.OnBeforeLeaveLocation(leaveLocationEventArgs);
                    newLocationMap.Location.OnBeforeEnterLocation(enterLocationEventArgs);

                    if (!newLocationMap.Location.IsLocked)
                    {
                        if (!newLocationMap.Location.IsClosed)
                        {
                            var oldLocation = Universe.ActiveLocation;
                            oldLocation.OnLeaveLocation(leaveLocationEventArgs);

                            Universe.ActiveLocation = newLocationMap.Location;

                            ObjectHandler.ClearActiveObjectIfNotInInventory();

                            Universe.ActiveLocation.OnEnterLocation(enterLocationEventArgs);

                            PrintingSubsystem.ActiveLocation(Universe.ActiveLocation, Universe.LocationMap);

                            oldLocation.OnAfterLeaveLocation(leaveLocationEventArgs);
                            Universe.ActiveLocation.OnAfterEnterLocation(enterLocationEventArgs);

                            return true;
                        }

                        return PrintingSubsystem.WayIsClosed(newLocationMap.Location);
                    }

                    return PrintingSubsystem.WayIsLocked(newLocationMap.Location);
                }
                catch (LeaveLocationException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
                catch (EnterLocationException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.Resource(BaseDescriptions.NO_WAY);
        }

        return PrintingSubsystem.Resource(BaseDescriptions.NO_WAY);
    }
}