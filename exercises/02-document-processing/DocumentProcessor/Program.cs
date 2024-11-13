using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var analyzer = new AzureDocumentAnalyzer(
            configuration["AzureFormRecognizer:Endpoint"],
            configuration["AzureFormRecognizer:Key"]);

        var extractor = new TextExtractor(analyzer);
        var normalizer = new DocumentNormalizer();

        // Example usage
        var metadata = new DocumentMetadata
        {
            Title = "Sample Document",
            Author = "John Doe",
            CreationDate = DateTime.UtcNow,
            DocumentType = "PDF",
            Language = "en"
        };

        var processedDoc = await extractor.ProcessDocumentAsync(
            "path/to/document.pdf", 
            metadata);

        var normalizedText = normalizer.NormalizeText(processedDoc.Content);
        normalizedText = normalizer.RemoveSpecialCharacters(normalizedText);

        Console.WriteLine($"Processed Document ID: {processedDoc.Id}");
        Console.WriteLine($"Normalized Content: {normalizedText.Substring(0, 100)}...");
    }
}