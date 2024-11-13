public class QueryOptimizer
{
    private readonly IConfiguration _configuration;

    public QueryOptimizer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SearchQuery OptimizeQuery(string searchText, int resultCount = 3)
    {
        return new SearchQuery
        {
            SearchText = searchText,
            ResultCount = resultCount,
            Embedding = GenerateEmbedding(searchText)
        };
    }

    private float[] GenerateEmbedding(string text)
    {
        var openAIClient = new OpenAIClient(
            new Uri(_configuration["Azure:OpenAI:Endpoint"]),
            new AzureKeyCredential(_configuration["Azure:OpenAI:Key"]));

        var response = openAIClient.GetEmbeddings(
            _configuration["Azure:OpenAI:DeploymentName"],
            new EmbeddingsOptions(text));

        return response.Value.Data[0].Embedding.ToArray();
    }
}