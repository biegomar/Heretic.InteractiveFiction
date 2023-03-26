using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public sealed partial class Character
{
    public event EventHandler<ContainerObjectEventArgs>? BeforeTalk;
    public event EventHandler<ContainerObjectEventArgs>? Talk;
    public event EventHandler<ContainerObjectEventArgs>? AfterTalk;
    
    public event EventHandler<ConversationEventArgs>? Ask;
    public event EventHandler<ConversationEventArgs>? Say;
    
    public void OnBeforeTalk(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeTalk;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnTalk(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Talk;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public void OnAfterTalk(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterTalk;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public void OnAsk(ConversationEventArgs eventArgs)
    {
        var localEventHandler = this.Ask;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            var itemName = string.Empty;
            if (eventArgs.Item != null)
            {
                itemName = ArticleHandler.GetNameWithArticleForObject(eventArgs.Item, GrammarCase.Dative);
                
            }
            
            throw new AskException(itemName);
        }
    }

    public void OnSay(ConversationEventArgs eventArgs)
    {
        var localEventHandler = this.Say;
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