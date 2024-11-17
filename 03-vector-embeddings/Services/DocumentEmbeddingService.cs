using System.Collections.Generic;
using System.Threading.Tasks;

public class DocumentEmbeddingService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly List<DocumentEmbedding> _embeddingStore;

    public DocumentEmbeddingService(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
        _embeddingStore = new List<DocumentEmbedding>();
    }

    public async Task AddDocumentChunksAsync(List<DocumentChunk> chunks)
    {
        var texts = chunks.Select(c => c.Content);
        var embeddings = await _embeddingService.GenerateEmbeddingsAsync(texts);

        for (int i = 0; i < chunks.Count; i++)
        {
            var documentEmbedding = new DocumentEmbedding
            {
                Id = chunks[i].Id,
                Content = chunks[i].Content,
                Embedding = embeddings[i],
                Metadata = chunks[i].Metadata
            };
            _embeddingStore.Add(documentEmbedding);
        }
    }

    public List<DocumentEmbedding> GetAllEmbeddings()
    {
        return _embeddingStore;
    }
} 