using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class SearchDialog : ComponentDialog
{
    private readonly ISearchService _searchService;

    public SearchDialog(ISearchService searchService)
        : base(nameof(SearchDialog))
    {
        _searchService = searchService;

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
            SearchStepAsync,
            ShowResultsStepAsync
        }));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> SearchStepAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var query = stepContext.Context.Activity.Text;
        var searchResults = await _searchService.SearchAsync(query);
        stepContext.Values["searchResults"] = searchResults;

        return await stepContext.NextAsync(cancellationToken: cancellationToken);
    }

    private async Task<DialogTurnResult> ShowResultsStepAsync(
        WaterfallStepContext stepContext,
        CancellationToken cancellationToken)
    {
        var searchResults = (List<SearchResult>)stepContext.Values["searchResults"];

        if (searchResults.Any())
        {
            var responseText = "Here are the most relevant results:\n\n";
            foreach (var result in searchResults)
            {
                responseText += $"- {result.Content}\n";
                responseText += $"  Source: {result.Metadata["document_type"]}, Score: {result.Score:F2}\n\n";
            }
            await stepContext.Context.SendActivityAsync(responseText, cancellationToken: cancellationToken);
        }
        else
        {
            await stepContext.Context.SendActivityAsync(
                "I couldn't find any relevant information for your query.",
                cancellationToken: cancellationToken);
        }

        return await stepContext.EndDialogAsync(null, cancellationToken);
    }
} 