using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    Task<List<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts);
} 