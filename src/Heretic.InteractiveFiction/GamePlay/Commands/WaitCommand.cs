using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record WaitCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem): ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        try
        {
            var containerObjectEventArgs = new ContainerObjectEventArgs()
                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

            Universe.ActiveLocation.OnWait(containerObjectEventArgs);

            return true;
        }
        catch (WaitException ex)
        {
            return PrintingSubsystem.Resource(ex.Message);
        }
    }
}