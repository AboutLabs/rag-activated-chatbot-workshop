using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class RagDialog : ComponentDialog
{
    private readonly IDocumentProcessor _documentProcessor;
    private readonly IEmbeddingService _embeddingService;
    private readonly ISearchService _searchService;
    private readonly IStatePropertyAccessor<RagBotState> _botStateAccessor;

    public RagDialog(
        ConversationState conversationState,
        IDocumentProcessor documentProcessor,
        IEmbeddingService embeddingService,
        ISearchService searchService)
        : base(nameof(RagDialog))
    {
        _documentProcessor = documentProcessor;
        _embeddingService = embeddingService;
        _searchService = searchService;
        _botStateAccessor = conversationState.CreateProperty<RagBotState>("RagBotState");

        var waterfallSteps = new WaterfallStep[]
        {
            InitialStepAsync,
            HandleInputAsync,
            FinalStepAsync
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> InitialStepAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var state = await _botStateAccessor.GetAsync(
            stepContext.Context,
            () => new RagBotState(),
            cancellationToken);

        if (stepContext.Context.Activity.Attachments?.Count > 0)
        {
            state.CurrentMode = "document";
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        var messageText = stepContext.Context.Activity.Text?.ToLowerInvariant();
        if (messageText == "upload" || messageText == "document")
        {
            state.CurrentMode = "document";
            await stepContext.Context.SendActivityAsync(
                "Please upload a document or send some text to process.",
                cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        state.CurrentMode = "chat";
        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> HandleInputAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var state = await _botStateAccessor.GetAsync(
            stepContext.Context,
            () => new RagBotState(),
            cancellationToken);

        if (state.CurrentMode == "document")
        {
            return await HandleDocumentAsync(stepContext, cancellationToken);
        }
        else
        {
            return await HandleQueryAsync(stepContext, cancellationToken);
        }
    }

    private async Task<DialogTurnResult> HandleDocumentAsync(
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

        var embeddings = new List<DocumentEmbedding>();
        foreach (var chunk in chunks)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk.Content);
            embeddings.Add(new DocumentEmbedding
            {
                Id = chunk.Id,
                Content = chunk.Content,
                Embedding = embedding,
                Metadata = chunk.Metadata
            });
        }

        await _searchService.IndexDocumentEmbeddingsAsync(embeddings);
        
        await stepContext.Context.SendActivityAsync(
            $"Processed and indexed {chunks.Count} chunks from the document.",
            cancellationToken: cancellationToken);

        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> HandleQueryAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var query = stepContext.Context.Activity.Text;
        var searchResults = await _searchService.SearchAsync(query);

        if (searchResults.Any())
        {
            var responseText = "Based on the available documents:\n\n";
            foreach (var result in searchResults)
            {
                responseText += $"- {result.Content}\n";
                responseText += $"  Source: {result.Metadata["document_type"]}, Score: {result.Score:F2}\n\n";
            }
            await stepContext.Context.SendActivityAsync(responseText, cancellationToken: cancellationToken);
        }
        else
        {
            await stepContext.Context.SendActivityAsync(
                "I couldn't find any relevant information for your query.",
                cancellationToken: cancellationToken);
        }

        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        return await stepContext.EndDialogAsync(null, cancellationToken);
    }
} 