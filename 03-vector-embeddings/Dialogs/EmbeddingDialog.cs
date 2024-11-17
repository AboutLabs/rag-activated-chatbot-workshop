using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

public class EmbeddingDialog : ComponentDialog
{
    private readonly IDocumentProcessor _documentProcessor;
    private readonly DocumentEmbeddingService _embeddingService;

    public EmbeddingDialog(
        IDocumentProcessor documentProcessor,
        DocumentEmbeddingService embeddingService)
        : base(nameof(EmbeddingDialog))
    {
        _documentProcessor = documentProcessor;
        _embeddingService = embeddingService;

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
            ProcessDocumentStepAsync,
            GenerateEmbeddingsStepAsync,
            ShowResultStepAsync
        }));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> ProcessDocumentStepAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var activity = stepContext.Context.Activity;
        List<DocumentChunk> chunks;

        if (activity.Attachments?.Count > 0)
        {
            chunks = new List<DocumentChunk>();
            foreach (var attachment in activity.Attachments)
            {
                using (var stream = await new System.Net.Http.HttpClient().GetStreamAsync(attachment.ContentUrl))
                {
                    chunks.AddRange(await _documentProcessor.ProcessDocumentAsync(stream, attachment.Name));
                }
            }
        }
        else
        {
            chunks = await _documentProcessor.ProcessTextAsync(activity.Text);
        }

        stepContext.Values["chunks"] = chunks;
        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> GenerateEmbeddingsStepAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var chunks = (List<DocumentChunk>)stepContext.Values["chunks"];
        await _embeddingService.AddDocumentChunksAsync(chunks);
        
        await stepContext.Context.SendActivityAsync(
            $"Generated embeddings for {chunks.Count} chunks.",
            cancellationToken: cancellationToken);

        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> ShowResultStepAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var totalEmbeddings = _embeddingService.GetAllEmbeddings().Count;
        await stepContext.Context.SendActivityAsync(
            $"Total documents with embeddings: {totalEmbeddings}",
            cancellationToken: cancellationToken);

        return await stepContext.EndDialogAsync(null, cancellationToken);
    }
} 