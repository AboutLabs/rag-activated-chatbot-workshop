using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EchoBotDemo
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        public AdapterWithErrorHandler(
            IConfiguration configuration,
            ILogger<IBotFrameworkHttpAdapter> logger)
            : base(configuration, null, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                logger.LogError(exception, 
                    $"Error processing activity: {exception.Message}");

                await turnContext.SendActivityAsync(
                    "Sorry, something went wrong.");

                await turnContext.TraceActivityAsync(
                    "OnTurnError Trace",
                    exception.Message,
                    "https://www.botframework.com/schemas/error",
                    "TurnError");
            };
        }
    }
}