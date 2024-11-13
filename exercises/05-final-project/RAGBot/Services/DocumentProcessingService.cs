using Azure.AI.FormRecognizer.DocumentAnalysis;

public class DocumentProcessingService
{
    private readonly DocumentAnalysisClient _client;

    public DocumentProcessingService(string endpoint, string key)
    {
        _client = new DocumentAnalysisClient(
            new Uri(endpoint), 
            new AzureKeyCredential(key));
    }

    public async Task<string> ExtractTextAsync(Stream documentStream)
    {
        var operation = await _client.AnalyzeDocumentAsync(
            WaitUntil.Completed, 
            "prebuilt-document", 
            documentStream);

        var result = operation.Value;
        return string.Join(" ", result.Paragraphs.Select(p => p.Content));
    }

    public async Task<List<Document>> ProcessDocumentsAsync(
        string folderPath)
    {
        var documents = new List<Document>();
        var files = Directory.GetFiles(folderPath);

        foreach (var file in files)
        {
            using var stream = File.OpenRead(file);
            var text = await ExtractTextAsync(stream);
            
            documents.Add(new Document
            {
                Id = Guid.NewGuid().ToString(),
                Content = text,
                Source = file,
                ProcessedDate = DateTime.UtcNow
            });
        }

        return documents;
    }
}