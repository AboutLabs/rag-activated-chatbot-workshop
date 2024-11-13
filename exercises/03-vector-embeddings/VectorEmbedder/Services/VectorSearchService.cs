using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;

public class VectorSearchService
{
    private readonly SearchClient _searchClient;
    private readonly EmbeddingService _embeddingService;

    public VectorSearchService(
        IConfiguration configuration,
        EmbeddingService embeddingService)
    {
        _searchClient = new SearchClient(
            new Uri(configuration["Azure:Search:Endpoint"]),
            configuration["Azure:Search:IndexName"],
            new Azure.AzureKeyCredential(configuration["Azure:Search:Key"]));
        _embeddingService = embeddingService;
    }

    public async Task<IList<SearchResult>> SearchSimilarDocumentsAsync(
        string query, 
        int limit = 5)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingsAsync(query);

        var searchOptions = new SearchOptions
        {
            Size = limit,
            Select = { "id", "content", "metadata" },
            VectorSearch = new VectorSearchOptions
            {
                Queries = { new VectorizedQuery(queryEmbedding) { KNearestNeighborsCount = limit } }
            }
        };

        var response = await _searchClient.SearchAsync<Document>(null, searchOptions);
        
        return await response.Value.GetResultsAsync()
            .Select(result => new SearchResult
            {
                Id = result.Document.Id,
                Content = result.Document.Content,
                Score = result.Score ?? 0,
                Metadata = result.Document.Metadata
            })
            .ToListAsync();
    }
}