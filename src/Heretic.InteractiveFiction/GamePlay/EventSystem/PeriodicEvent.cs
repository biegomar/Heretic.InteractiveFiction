using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem;

public class PeriodicEvent
{
    private int activeCounter = 0;

    public bool Active { get; set; }

    public int MinDistanceBetweenEvents { get; init; }
    public int MaxDistanceBetweenEvents { get; init; }
    public int AverageDistanceBetweenEvents { get; init; }
    public string Phrases { get; init; } = string.Empty;

    public void RaiseEvent(object sender, PeriodicEventArgs eventArgs)
    {
        if (this.Active)
        {
            if (this.activeCounter >= this.MinDistanceBetweenEvents)
            {
                if (this.activeCounter >= this.MaxDistanceBetweenEvents)
                {
                    this.activeCounter = 0;
                    eventArgs.Message = this.GetRandomPhrase();
                    return;
                }

                Random rand = new Random();
                if (rand.Next(0, this.AverageDistanceBetweenEvents + 1) == this.AverageDistanceBetweenEvents)
                {
                    this.activeCounter = 0;
                    eventArgs.Message = this.GetRandomPhrase();
                    return;
                }
            }

            this.activeCounter++;
        }
    }

    private string GetRandomPhrase()
    {
        var phrases = this.Phrases.Split("|");

        var rand = new Random();
        var index = rand.Next(phrases.Length);

        return phrases[index];
    }
}
