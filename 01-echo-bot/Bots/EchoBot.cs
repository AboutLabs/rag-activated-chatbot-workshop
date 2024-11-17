using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class EchoBot : ActivityHandler
{
    private readonly BotState _conversationState;
    private readonly BotState _userState;
    private readonly Dialog _dialog;

    public EchoBot(ConversationState conversationState, UserState userState, Dialog dialog)
    {
        _conversationState = conversationState;
        _userState = userState;
        _dialog = dialog;
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
    }

    protected override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        await base.OnTurnAsync(turnContext, cancellationToken);
        await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
    }
} 