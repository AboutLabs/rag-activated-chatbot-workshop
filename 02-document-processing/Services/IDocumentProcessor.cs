using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IDocumentProcessor
{
    Task<List<DocumentChunk>> ProcessDocumentAsync(Stream documentStream, string fileName);
    Task<List<DocumentChunk>> ProcessTextAsync(string text);
} 