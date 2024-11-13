using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Core;

public class AzureDocumentAnalyzer
{
    private readonly DocumentAnalysisClient _client;

    public AzureDocumentAnalyzer(string endpoint, string key)
    {
        _client = new DocumentAnalysisClient(
            new Uri(endpoint), 
            new AzureKeyCredential(key));
    }

    public async Task<string> ExtractTextFromFileAsync(Stream documentStream)
    {
        var operation = await _client.AnalyzeDocumentAsync(
            WaitUntil.Completed, 
            "prebuilt-document", 
            documentStream);

        var result = operation.Value;
        return string.Join(" ", result.Paragraphs.Select(p => p.Content));
    }
}