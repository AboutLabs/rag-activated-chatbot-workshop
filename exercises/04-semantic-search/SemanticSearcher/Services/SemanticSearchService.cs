using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;

public class SemanticSearchService
{
    private readonly SearchClient _searchClient;
    private readonly string _semanticConfigurationName;

    public SemanticSearchService(IConfiguration configuration)
    {
        _searchClient = new SearchClient(
            new Uri(configuration["Azure:Search:Endpoint"]),
            configuration["Azure:Search:IndexName"],
            new Azure.AzureKeyCredential(configuration["Azure:Search:Key"]));
        _semanticConfigurationName = configuration["Azure:Search:SemanticConfig"];
    }

    public async Task<SearchResults<SearchDocument>> SearchAsync(
        SearchQuery query)
    {
        var options = new SearchOptions
        {
            Size = query.ResultCount,
            QueryType = SearchQueryType.Semantic,
            SemanticConfigurationName = _semanticConfigurationName,
            QueryLanguage = QueryLanguage.EnUs,
            QueryAnswer = QueryAnswerType.Extractive,
            QueryCaption = QueryCaptionType.Extractive,
            VectorSearch = new VectorSearchOptions
            {
                Queries = { new VectorizedQuery(query.Embedding) 
                { 
                    KNearestNeighborsCount = query.ResultCount,
                    Fields = { "contentVector" }
                }}
            }
        };

        options.Select.AddRange(new[] 
        { 
            "id", 
            "content", 
            "title", 
            "metadata" 
        });

        return await _searchClient.SearchAsync<SearchDocument>(
            query.SearchText, 
            options);
    }
}