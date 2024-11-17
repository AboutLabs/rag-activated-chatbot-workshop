using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISearchService
{
    Task IndexDocumentEmbeddingsAsync(List<DocumentEmbedding> embeddings);
    Task<List<SearchResult>> SearchAsync(string query, int topK = 3);
    Task<List<SearchResult>> SearchSimilarAsync(float[] queryEmbedding, int topK = 3);
} 