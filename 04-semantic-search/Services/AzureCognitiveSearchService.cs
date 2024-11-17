using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AzureCognitiveSearchService : ISearchService
{
    private readonly SearchIndexClient _indexClient;
    private readonly SearchClient _searchClient;
    private readonly IEmbeddingService _embeddingService;
    private const string IndexName = "documents";

    public AzureCognitiveSearchService(
        string endpoint,
        string key,
        IEmbeddingService embeddingService)
    {
        _indexClient = new SearchIndexClient(
            new Uri(endpoint),
            new AzureKeyCredential(key));
        
        _searchClient = new SearchClient(
            new Uri(endpoint),
            IndexName,
            new AzureKeyCredential(key));
        
        _embeddingService = embeddingService;
        
        EnsureIndexExists().Wait();
    }

    private async Task EnsureIndexExists()
    {
        if (!await _indexClient.GetIndexNamesAsync().ToListAsync().ContinueWith(t => t.Result.Contains(IndexName)))
        {
            var vectorConfig = new VectorSearchConfiguration("vector-config", 1536);
            
            var fields = new List<SearchField>
            {
                new SearchField("id", SearchFieldDataType.String) { IsKey = true },
                new SearchField("content", SearchFieldDataType.String) { IsSearchable = true, IsFilterable = true },
                new SearchField("embedding", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = 1536,
                    VectorSearchConfiguration = "vector-config"
                },
                new SearchField("metadata", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsFilterable = true }
            };

            var index = new SearchIndex(IndexName, fields) 
            { 
                VectorSearch = new VectorSearch { Algorithms = { vectorConfig } }
            };

            await _indexClient.CreateIndexAsync(index);
        }
    }

    public async Task IndexDocumentEmbeddingsAsync(List<DocumentEmbedding> embeddings)
    {
        var batch = new List<SearchDocument>();

        foreach (var doc in embeddings)
        {
            var searchDoc = new SearchDocument
            {
                ["id"] = doc.Id,
                ["content"] = doc.Content,
                ["embedding"] = doc.Embedding,
                ["metadata"] = doc.Metadata.Select(kvp => $"{kvp.Key}:{kvp.Value}").ToList()
            };
            batch.Add(searchDoc);
        }

        await _searchClient.UploadDocumentsAsync(batch);
    }

    public async Task<List<SearchResult>> SearchAsync(string query)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
        return await SearchSimilarAsync(queryEmbedding);
    }

    public async Task<List<SearchResult>> SearchSimilarAsync(float[] queryEmbedding, int topK = 3)
    {
        var vectorQuery = new VectorizedQuery(queryEmbedding)
        {
            KNearestNeighborsCount = topK,
            Fields = { "embedding" }
        };

        var response = await _searchClient.SearchAsync<SearchDocument>(
            searchText: null,
            new SearchOptions 
            { 
                VectorQueries = { vectorQuery },
                Size = topK,
                Select = { "id", "content", "metadata" }
            });

        var results = new List<SearchResult>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            var metadata = ((string[])result.Document["metadata"])
                .Select(s => s.Split(':'))
                .ToDictionary(parts => parts[0], parts => parts[1]);

            results.Add(new SearchResult
            {
                Id = result.Document["id"].ToString(),
                Content = result.Document["content"].ToString(),
                Score = result.Score ?? 0,
                Metadata = metadata
            });
        }

        return results;
    }
} 