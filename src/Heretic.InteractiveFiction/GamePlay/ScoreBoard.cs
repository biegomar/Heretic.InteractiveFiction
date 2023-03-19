using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class ScoreBoard
{
    private readonly IList<string> scored = new List<string>
    {
        BaseDescriptions.YOU_SCORED_I,
        BaseDescriptions.YOU_SCORED_II,
        BaseDescriptions.YOU_SCORED_III
    };
    
    private readonly IDictionary<string, int> availableScores;
    private readonly IDictionary<string, int> obtainedScores;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly Random random;

    public int MaxScore => this.availableScores.Sum(kv => kv.Value);
    
    public int Score => this.obtainedScores.Sum(kv => kv.Value);

    public ScoreBoard(IPrintingSubsystem printingSubsystem)
    {
        this.availableScores = new Dictionary<string, int>();
        this.obtainedScores = new Dictionary<string, int>();
        this.random = new Random();
        this.printingSubsystem = printingSubsystem;
    }

    public void RegisterScore(string key, int value)
    {
        if (!this.availableScores.ContainsKey(key))
        {
            this.availableScores.Add(key, value);
        }
    }

    public void WinScore(string key)
    {
        if (!this.obtainedScores.ContainsKey(key) && this.availableScores.ContainsKey(key))
        {
            this.obtainedScores.Add(key, this.availableScores[key]);
            this.PrintThatScoreWasObtained();
        }
    }

    public bool PrintScore()
    {
        return printingSubsystem.Score(this.Score, this.MaxScore);
    }
    
    private void PrintThatScoreWasObtained()
    {
        this.printingSubsystem.ForegroundColor = TextColor.Magenta;
        this.printingSubsystem.Resource(scored[this.random.Next(0, scored.Count)]);
        this.printingSubsystem.ResetColors();
    }
}