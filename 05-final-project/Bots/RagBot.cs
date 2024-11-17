using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

public class RagBot : ActivityHandler
{
    private readonly BotState _conversationState;
    private readonly BotState _userState;
    private readonly Dialog _dialog;
    private readonly IStatePropertyAccessor<RagBotState> _botStateAccessor;

    public RagBot(
        ConversationState conversationState,
        UserState userState,
        RagDialog dialog)
    {
        _conversationState = conversationState;
        _userState = userState;
        _dialog = dialog;
        _botStateAccessor = _conversationState.CreateProperty<RagBotState>("RagBotState");
    }

    protected override async Task OnMessageActivityAsync(
        ITurnContext<IMessageActivity> turnContext,
        CancellationToken cancellationToken)
    {
        var state = await _botStateAccessor.GetAsync(
            turnContext,
            () => new RagBotState(),
            cancellationToken);

        await _dialog.RunAsync(
            turnContext,
            _conversationState.CreateProperty<DialogState>("DialogState"),
            cancellationToken);
    }

    protected override async Task OnTurnAsync(
        ITurnContext turnContext,
        CancellationToken cancellationToken)
    {
        await base.OnTurnAsync(turnContext, cancellationToken);
        await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
    }
} 