using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record ToBeCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        var subject = adventureEvent.ObjectOne;

        if (subject != default)
        {
            ObjectHandler.StoreAsActiveObject(subject);

            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs()
                {
                    ExternalItemKey = adventureEvent.UnidentifiedSentenceParts.FirstOrDefault(),
                    OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage
                };
                subject.OnToBe(containerObjectEventArgs);

                return true;
            }
            catch (ToBeException ex)
            {
                return PrintingSubsystem.Resource(ex.Message);
            }
        }

        return PrintingSubsystem.ItemNotVisible();
    }
}