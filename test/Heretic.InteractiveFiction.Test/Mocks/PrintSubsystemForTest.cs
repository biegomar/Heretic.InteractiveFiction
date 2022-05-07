using System;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.Test.Mocks;

public class PrintSubsystemForTest: BaseConsolePrintingSubsystem
{
    public override bool Opening()
    {
        Console.WriteLine("Opening");
        return true;
    }

    public override bool Closing()
    {
        Console.WriteLine("Closing");
        return true;
    }
    
    public override bool TitleAndScore(int score, int maxScore)
    {
        Console.Title = $"{string.Format(BaseDescriptions.SCORE, score, maxScore)}";
        return true;
    }
    

    public override bool Credits()
    {
        Console.WriteLine("Credits");
        return true;
    }
}