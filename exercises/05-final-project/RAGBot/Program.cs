var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<DocumentProcessingService>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<AnswerGenerationService>();
builder.Services.AddTransient<IBot, RagBot>();

var app = builder.Build();

app.UseHttpsRedirection()
   .UseWebSockets()
   .UseRouting()
   .UseAuthorization()
   .UseEndpoints(endpoints =>
   {
       endpoints.MapControllers();
   });

app.Run();