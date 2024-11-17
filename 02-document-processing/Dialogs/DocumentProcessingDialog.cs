using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

public class DocumentProcessingDialog : ComponentDialog
{
    private readonly IDocumentProcessor _documentProcessor;

    public DocumentProcessingDialog(IDocumentProcessor documentProcessor)
        : base(nameof(DocumentProcessingDialog))
    {
        _documentProcessor = documentProcessor;

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
            ProcessDocumentStepAsync,
            ShowResultStepAsync
        }));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> ProcessDocumentStepAsync(
        WaterfallStepContext stepContext, 
        CancellationToken cancellationToken)
    {
        var activity = stepContext.Context.Activity;
        
        if (activity.Attachments?.Count > 0)
        {
            foreach (var attachment in activity.Attachments)
            {
                using (var stream = await new System.Net.Http.HttpClient().GetStreamAsync(attachment.ContentUrl))
                {
                    var chunks = await _documentProcessor.ProcessDocumentAsync(stream, attachment.Name);
                    stepContext.Values["chunks"] = chunks;
                }
            }
            await stepContext.Context.SendActivityAsync("Document processed successfully!");
        }
        else
        {
            var chunks = await _documentProcessor.ProcessTextAsync(activity.Text);
            stepContext.Values["chunks"] = chunks;
            await stepContext.Context.SendActivityAsync("Text processed successfully!");
        }

        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> ShowResultStepAsync(
        WaterfallStepContext stepContext, 
        CancellationToken cancellationToken)
    {
        var chunks = (List<DocumentChunk>)stepContext.Values["chunks"];
        await stepContext.Context.SendActivityAsync($"Created {chunks.Count} chunks from the document.");
        return await stepContext.EndDialogAsync(null, cancellationToken);
    }
} 