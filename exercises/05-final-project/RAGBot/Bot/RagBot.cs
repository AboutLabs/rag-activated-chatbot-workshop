using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

public class RagBot : ActivityHandler
{
    private readonly DocumentProcessingService _documentService;
    private readonly EmbeddingService _embeddingService;
    private readonly SearchService _searchService;
    private readonly AnswerGenerationService _answerService;

    public RagBot(
        DocumentProcessingService documentService,
        EmbeddingService embeddingService,
        SearchService searchService,
        AnswerGenerationService answerService)
    {
        _documentService = documentService;
        _embeddingService = embeddingService;
        _searchService = searchService;
        _answerService = answerService;
    }

    protected override async Task OnMessageActivityAsync(
        ITurnContext<IMessageActivity> turnContext,
        CancellationToken cancellationToken)
    {
        var userQuery = turnContext.Activity.Text;
        
        // Generate embedding for user query
        var queryEmbedding = await _embeddingService
            .GenerateEmbeddingsAsync(userQuery);

        // Search for relevant documents
        var searchResults = await _searchService
            .SearchSimilarDocumentsAsync(queryEmbedding);

        // Generate answer using RAG
        var answer = await _answerService
            .GenerateAnswerAsync(userQuery, searchResults);

        await turnContext.SendActivityAsync(
            MessageFactory.Text(answer.Response), 
            cancellationToken);
    }

    protected override async Task OnMembersAddedAsync(
        IList<ChannelAccount> membersAdded,
        ITurnContext<IConversationUpdateActivity> turnContext,
        CancellationToken cancellationToken)
    {
        foreach (var member in membersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text(
                        "Welcome! I'm a RAG-enabled chatbot. How can I help you?"),
                    cancellationToken);
            }
        }
    }
}