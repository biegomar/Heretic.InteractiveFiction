using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Resources;


namespace Heretic.InteractiveFiction.Objects;

public sealed class Character : AHereticObject
{
    public string TalkDescription { get; set; }
    public string StandardPhrases { get; set; }
    public bool Talked { get; set; }

    public event EventHandler<ContainerObjectEventArgs> AfterTalk;
    public event EventHandler<ConversationEventArgs> Ask;
    public event EventHandler<ConversationEventArgs> Say;

    public Character() : base()
    {
        this.IsPickAble = false;
    }

    public string Talk()
    {
        if (!Talked && !string.IsNullOrEmpty(this.TalkDescription))
        {
            this.Talked = true;
            return this.TalkDescription;
        }
        else if (!string.IsNullOrEmpty(this.StandardPhrases))
        {
            return GetRandomPhrase();
        }

        return string.Empty;
    }

    private string GetRandomPhrase()
    {
        var phrases = StandardPhrases.Split("|");

        var rand = new Random();
        var index = rand.Next(phrases.Length);

        return phrases[index];
    }

    private string GenderSpeech()
    {
        return this.Grammar.Gender switch
        {
            Genders.Female => BaseDescriptions.GENDER_FEMALE,
            Genders.Male => BaseDescriptions.GENDER_MALE,
            _ => BaseDescriptions.GENDER_UNKNOWN
        };
    }

    protected override string GetVariationOfHereSingle()
    {
        return string.Format(BaseDescriptions.INVENTORY_TEMPLATE, this.GenderSpeech());
    }

    protected override string GetVariationOfYouSee(int itemCount)
    {
        return this.Grammar.Gender switch
        {
            Genders.Female => BaseDescriptions.YOU_SEE_FEMALE,
            Genders.Male => BaseDescriptions.YOU_SEE_MALE,
            _ => BaseDescriptions.YOU_SEE
        };
    }

    public void OnAfterTalk(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterTalk;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnAsk(ConversationEventArgs eventArgs)
    {
        EventHandler<ConversationEventArgs> localEventHandler = this.Ask;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new AskException(eventArgs.Item.Name);
        }
    }

    public void OnSay(ConversationEventArgs eventArgs)
    {
        EventHandler<ConversationEventArgs> localEventHandler = this.Say;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new SayException(BaseDescriptions.NO_ANSWER_EXCEPTION);
        }
    }
}
