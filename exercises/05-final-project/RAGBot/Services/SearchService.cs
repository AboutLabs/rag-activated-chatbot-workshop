using Azure.Search.Documents;
using Azure.Search.Documents.Models;

public class SearchService
{
    private readonly SearchClient _searchClient;

    public SearchService(string endpoint, string indexName, string key)
    {
        _searchClient = new SearchClient(
            new Uri(endpoint),
            indexName,
            new AzureKeyCredential(key));
    }

    public async Task<List<SearchResult>> SearchSimilarDocumentsAsync(
        float[] queryEmbedding,
        int limit = 3)
    {
        var searchOptions = new SearchOptions
        {
            Size = limit,
            Select = { "id", "content", "source" },
            VectorSearch = new VectorSearchOptions
            {
                Queries = { 
                    new VectorizedQuery(queryEmbedding) 
                    { 
                        KNearestNeighborsCount = limit,
                        Fields = { "contentVector" }
                    }
                }
            }
        };

        var response = await _searchClient.SearchAsync<SearchDocument>(
            null, 
            searchOptions);

        return await ProcessSearchResultsAsync(response);
    }

    private async Task<List<SearchResult>> ProcessSearchResultsAsync(
        Response<SearchResults<SearchDocument>> searchResponse)
    {
        var results = new List<SearchResult>();
        
        await foreach (var result in searchResponse.Value.GetResultsAsync())
        {
            results.Add(new SearchResult
            {
                Id = result.Document["id"].ToString(),
                Content = result.Document["content"].ToString(),
                Source = result.Document["source"].ToString(),
                Score = result.Score ?? 0
            });
        }

        return results;
    }
}