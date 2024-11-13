public class TextExtractor
{
    private readonly AzureDocumentAnalyzer _analyzer;

    public TextExtractor(AzureDocumentAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }

    public async Task<ProcessedDocument> ProcessDocumentAsync(
        string filePath, 
        DocumentMetadata metadata)
    {
        using var stream = File.OpenRead(filePath);
        var extractedText = await _analyzer.ExtractTextFromFileAsync(stream);

        return new ProcessedDocument
        {
            Id = Guid.NewGuid().ToString(),
            Content = extractedText,
            Metadata = metadata,
            ProcessedDate = DateTime.UtcNow
        };
    }
}