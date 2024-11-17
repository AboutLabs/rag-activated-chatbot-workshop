using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class AzureDocumentProcessor : IDocumentProcessor
{
    private readonly DocumentAnalysisClient _documentAnalysisClient;
    private readonly int _maxChunkSize;

    public AzureDocumentProcessor(string endpoint, string key, int maxChunkSize = 1000)
    {
        _documentAnalysisClient = new DocumentAnalysisClient(
            new Uri(endpoint), 
            new AzureKeyCredential(key));
        _maxChunkSize = maxChunkSize;
    }

    public async Task<List<DocumentChunk>> ProcessDocumentAsync(Stream documentStream, string fileName)
    {
        var operation = await _documentAnalysisClient.AnalyzeDocumentAsync(
            WaitUntil.Completed, 
            "prebuilt-layout", 
            documentStream);

        var result = operation.Value;
        var chunks = new List<DocumentChunk>();

        for (int i = 0; i < result.Pages.Count; i++)
        {
            var page = result.Pages[i];
            var pageContent = string.Join(" ", page.Lines.Select(l => l.Content));
            var textChunks = ChunkText(pageContent);

            chunks.AddRange(textChunks.Select(chunk => new DocumentChunk
            {
                Id = Guid.NewGuid().ToString(),
                Content = chunk,
                SourceDocument = fileName,
                PageNumber = i + 1,
                Metadata = new Dictionary<string, string>
                {
                    { "page_number", (i + 1).ToString() },
                    { "document_type", "pdf" }
                }
            }));
        }

        return chunks;
    }

    public async Task<List<DocumentChunk>> ProcessTextAsync(string text)
    {
        var chunks = ChunkText(text);
        return chunks.Select(chunk => new DocumentChunk
        {
            Id = Guid.NewGuid().ToString(),
            Content = chunk,
            SourceDocument = "text_input",
            PageNumber = 1,
            Metadata = new Dictionary<string, string>
            {
                { "document_type", "text" }
            }
        }).ToList();
    }

    private List<string> ChunkText(string text)
    {
        var chunks = new List<string>();
        var words = text.Split(' ');
        var currentChunk = new List<string>();
        var currentLength = 0;

        foreach (var word in words)
        {
            if (currentLength + word.Length + 1 > _maxChunkSize)
            {
                chunks.Add(string.Join(" ", currentChunk));
                currentChunk.Clear();
                currentLength = 0;
            }

            currentChunk.Add(word);
            currentLength += word.Length + 1;
        }

        if (currentChunk.Any())
        {
            chunks.Add(string.Join(" ", currentChunk));
        }

        return chunks;
    }
} 