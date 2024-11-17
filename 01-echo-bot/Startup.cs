using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Create storage
        var storage = new MemoryStorage();

        // Create state management
        var conversationState = new ConversationState(storage);
        var userState = new UserState(storage);

        // Create dialog
        var dialog = new MainDialog();

        // Add bot services
        services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
        services.AddSingleton<IStorage>(storage);
        services.AddSingleton<ConversationState>(conversationState);
        services.AddSingleton<UserState>(userState);
        services.AddSingleton<MainDialog>(dialog);
        services.AddTransient<IBot, EchoBot>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection()
           .UseRouting()
           .UseAuthorization()
           .UseEndpoints(endpoints =>
           {
                endpoints.MapControllers();
           });
    }
} 