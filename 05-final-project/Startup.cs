using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RAGChatbot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Create storage
            var storage = new MemoryStorage();

            // Create state management
            var conversationState = new ConversationState(storage);
            var userState = new UserState(storage);

            // Add document processing
            services.AddSingleton<IDocumentProcessor>(sp => 
                new AzureDocumentProcessor(
                    Configuration["AzureFormRecognizer:Endpoint"],
                    Configuration["AzureFormRecognizer:Key"],
                    maxChunkSize: 1000));

            // Add embedding service
            services.AddSingleton<IEmbeddingService>(sp => 
                new AzureOpenAIEmbedding(
                    Configuration["AzureOpenAI:Endpoint"],
                    Configuration["AzureOpenAI:Key"],
                    Configuration["AzureOpenAI:EmbeddingDeployment"]));

            // Add search service
            services.AddSingleton<ISearchService>(sp => 
                new AzureCognitiveSearchService(
                    Configuration["AzureSearch:Endpoint"],
                    Configuration["AzureSearch:Key"],
                    sp.GetRequiredService<IEmbeddingService>()));

            // Add bot services
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<IStorage>(storage);
            services.AddSingleton<ConversationState>(conversationState);
            services.AddSingleton<UserState>(userState);
            services.AddSingleton<RagDialog>();
            services.AddTransient<IBot, RagBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
} 