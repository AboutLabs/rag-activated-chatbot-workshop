using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

public class EmbeddingService
{
    private readonly OpenAIClient _client;
    private readonly string _deploymentName;

    public EmbeddingService(IConfiguration configuration)
    {
        _client = new OpenAIClient(
            new Uri(configuration["Azure:OpenAI:Endpoint"]),
            new Azure.AzureKeyCredential(configuration["Azure:OpenAI:Key"]));
        _deploymentName = configuration["Azure:OpenAI:EmbeddingDeployment"];
    }

    public async Task<float[]> GenerateEmbeddingsAsync(string text)
    {
        var response = await _client.GetEmbeddingsAsync(
            _deploymentName,
            new EmbeddingsOptions(text));

        return response.Value.Data[0].Embedding.ToArray();
    }

    public async Task<IList<float[]>> GenerateEmbeddingsBatchAsync(
        IEnumerable<string> texts)
    {
        var embeddings = new List<float[]>();
        foreach (var text in texts)
        {
            var embedding = await GenerateEmbeddingsAsync(text);
            embeddings.Add(embedding);
        }
        return embeddings;
    }
}