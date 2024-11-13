using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        var embeddingService = host.Services.GetRequiredService<EmbeddingService>();
        var searchService = host.Services.GetRequiredService<VectorSearchService>();

        // Example usage
        Console.WriteLine("Enter search query:");
        var query = Console.ReadLine();

        var results = await searchService.SearchSimilarDocumentsAsync(query);

        foreach (var result in results)
        {
            Console.WriteLine($"Document ID: {result.Id}");
            Console.WriteLine($"Content: {result.Content}");
            Console.WriteLine($"Score: {result.Score}");
            Console.WriteLine("-------------------");
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<EmbeddingService>();
                services.AddSingleton<VectorSearchService>();
            });
}