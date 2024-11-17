using Azure.AI.OpenAI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AzureOpenAIEmbedding : IEmbeddingService
{
    private readonly OpenAIClient _client;
    private readonly string _deploymentName;

    public AzureOpenAIEmbedding(string endpoint, string key, string deploymentName)
    {
        _client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
        _deploymentName = deploymentName;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var response = await _client.GetEmbeddingsAsync(_deploymentName, new EmbeddingsOptions(text));
        return response.Value.Data[0].Embedding.ToArray();
    }

    public async Task<List<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts)
    {
        var embeddings = new List<float[]>();
        foreach (var text in texts)
        {
            var embedding = await GenerateEmbeddingAsync(text);
            embeddings.Add(embedding);
        }
        return embeddings;
    }
} 