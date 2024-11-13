using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var searchService = host.Services.GetRequiredService<SemanticSearchService>();
        var queryOptimizer = host.Services.GetRequiredService<QueryOptimizer>();
        var resultProcessor = host.Services.GetRequiredService<SearchResultProcessor>();

        Console.WriteLine("Enter your search query:");
        var searchText = Console.ReadLine();

        var query = queryOptimizer.OptimizeQuery(searchText);
        var searchResults = await searchService.SearchAsync(query);
        var processedResults = resultProcessor.ProcessResults(searchResults);

        foreach (var result in processedResults)
        {
            Console.WriteLine($"\nTitle: {result.Title}");
            Console.WriteLine($"Score: {result.Score:F2}");
            
            if (result.Captions?.Any() == true)
            {
                Console.WriteLine("Captions:");
                foreach (var caption in result.Captions)
                {
                    Console.WriteLine($"- {caption}");
                }
            }

            if (result.Answers?.Any() == true)
            {
                Console.WriteLine("Answers:");
                foreach (var answer in result.Answers)
                {
                    Console.WriteLine($"- {answer}");
                }
            }
            
            Console.WriteLine("-------------------");
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<SemanticSearchService>();
                services.AddSingleton<QueryOptimizer>();
                services.AddSingleton<SearchResultProcessor>();
            });
}