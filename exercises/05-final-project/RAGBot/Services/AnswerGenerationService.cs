using Azure.AI.OpenAI;

public class AnswerGenerationService
{
    private readonly OpenAIClient _client;
    private readonly string _deploymentName;

    public AnswerGenerationService(
        string endpoint, 
        string key, 
        string deploymentName)
    {
        _client = new OpenAIClient(
            new Uri(endpoint),
            new AzureKeyCredential(key));
        _deploymentName = deploymentName;
    }

    public async Task<BotResponse> GenerateAnswerAsync(
        string query, 
        List<SearchResult> searchResults)
    {
        var context = string.Join("\n", 
            searchResults.Select(r => r.Content));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, 
                "You are a helpful assistant. Use the provided context to answer questions."),
            new ChatMessage(ChatRole.User, 
                $"Context:\n{context}\n\nQuestion: {query}")
        };

        var response = await _client.GetChatCompletionsAsync(
            _deploymentName,
            new ChatCompletionsOptions(messages));

        return new BotResponse
        {
            Response = response.Value.Choices[0].Message.Content,
            Sources = searchResults.Select(r => r.Source).ToList()
        };
    }
}