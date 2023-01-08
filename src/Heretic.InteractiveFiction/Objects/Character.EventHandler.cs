using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public sealed partial class Character
{
    public event EventHandler<ContainerObjectEventArgs> BeforeTalk;
    public event EventHandler<ContainerObjectEventArgs> Talk;
    public event EventHandler<ContainerObjectEventArgs> AfterTalk;
    
    public event EventHandler<ConversationEventArgs> Ask;
    public event EventHandler<ConversationEventArgs> Say;
    
    public void OnBeforeTalk(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.BeforeTalk;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnTalk(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.Talk;
        localEventHandler?.Invoke(this, eventArgs);
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
            var itemName = ArticleHandler.GetNameWithArticleForObject(eventArgs.Item, GrammarCase.Dative);
            throw new AskException(itemName);
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