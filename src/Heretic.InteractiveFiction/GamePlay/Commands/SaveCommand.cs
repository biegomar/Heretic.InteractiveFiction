using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record SaveCommand(IPrintingSubsystem PrintingSubsystem, HistoryAdministrator HistoryAdministrator) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        try
        {
            var history = new StringBuilder(HistoryAdministrator.All.Count);
            history.AppendJoin(Environment.NewLine, HistoryAdministrator.All.Take(HistoryAdministrator.All.Count - 1));

            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var combine = Path.Combine(docPath, BaseDescriptions.SAVE_NAME);
            using (var outputFile = new StreamWriter(combine))
            {
                outputFile.Write(history.ToString());
            }

            return PrintingSubsystem.FormattedResource(BaseDescriptions.GAME_SAVED, combine);
        }
        catch (Exception e)
        {
            PrintingSubsystem.Resource(e.Message);
            return false;
        }
    }
}