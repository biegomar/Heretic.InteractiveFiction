using System.Net;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

public record LoadCommand(IPrintingSubsystem PrintingSubsystem) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.UnidentifiedSentenceParts.Count == 1)
        {
            var file = adventureEvent.UnidentifiedSentenceParts.First();
            if (File.Exists(file))
            {
                PrintingSubsystem.Resource(BaseDescriptions.LOAD_GAME);
                throw new LoadException(file);
            }
            
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var combine = Path.Combine(docPath, file);
            if (File.Exists(combine))
            {
                PrintingSubsystem.Resource(BaseDescriptions.LOAD_GAME);
                throw new LoadException(combine);    
            }

            return PrintingSubsystem.Resource(BaseDescriptions.FILE_NOT_FOUND);
        }
        
        return PrintingSubsystem.Resource(BaseDescriptions.INSERT_FILENAME);
    }
}