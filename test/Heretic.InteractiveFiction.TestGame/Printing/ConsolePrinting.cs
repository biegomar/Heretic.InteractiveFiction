using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.Printing;

internal sealed class ConsolePrinting: BaseConsolePrintingSubsystem
{
    public override bool Opening()
    {
        if (!string.IsNullOrEmpty(MetaData.ASCII_TITLE))
        {
            Console.WriteLine(MetaData.ASCII_TITLE);
        }
        Console.WriteLine($@"{MetaData.TITLE} - {MetaData.VERSION}");
        this.ForegroundColor = TextColor.DarkCyan;
        this.Resource(BaseDescriptions.CHANGE_NAME);
        this.ResetColors();
        Console.Write(WordWrap(BaseDescriptions.HELLO_STRANGER, this.ConsoleWidth));
        Console.Write(WordWrap(Descriptions.OPENING, this.ConsoleWidth));
        Console.WriteLine();

        return true;
    }

    public override bool Closing()
    {
        Console.WriteLine(Descriptions.CLOSING);
        
        return true;
    }

    public override bool TitleAndScore(int score, int maxScore)
    {
        Console.Title = $"{string.Format(BaseDescriptions.SCORE, score, maxScore)}";
        return true;
    }

    public override bool Credits()
    {
        if (!string.IsNullOrEmpty(MetaData.ASCII_TITLE))
        {
            Console.WriteLine(MetaData.ASCII_TITLE);
        }
        Console.WriteLine($@"{MetaData.TITLE} - {MetaData.VERSION}");
        Console.WriteLine($@"Written by {MetaData.AUTHOR}");
        Console.WriteLine(MetaData.COPYRIGHT);
        Console.WriteLine();
        Console.WriteLine(GetVersionNumber());
        Console.WriteLine();

        return true;
    }
}