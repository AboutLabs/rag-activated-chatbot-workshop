using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

public class MainDialog : ComponentDialog
{
    public MainDialog() : base(nameof(MainDialog))
    {
        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
            InitialStepAsync,
            FinalStepAsync
        }));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var messageText = stepContext.Context.Activity.Text;
        var responseMessage = $"Echo: {messageText}";
        await stepContext.Context.SendActivityAsync(MessageFactory.Text(responseMessage), cancellationToken);
        return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.EndDialogAsync(null, cancellationToken);
    }
} 