using Azure.Identity;

public void ConfigureServices(IServiceCollection services)
{
    // ... existing service configurations ...

    // Add document processing
    services.AddSingleton<IDocumentProcessor>(sp => 
        new AzureDocumentProcessor(
            Configuration["AzureFormRecognizer:Endpoint"],
            Configuration["AzureFormRecognizer:Key"],
            maxChunkSize: 1000));

    // Update dialog registration
    services.AddSingleton<DocumentProcessingDialog>();
    services.AddSingleton<MainDialog>();
    services.AddTransient<IBot, DocumentBot>();
} 