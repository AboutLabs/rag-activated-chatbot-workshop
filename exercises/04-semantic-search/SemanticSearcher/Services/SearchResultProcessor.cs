public class SearchResultProcessor
{
    public List<SearchResult> ProcessResults(
        SearchResults<SearchDocument> searchResults)
    {
        var results = new List<SearchResult>();
        
        foreach (var result in searchResults.GetResults())
        {
            var searchResult = new SearchResult
            {
                Id = result.Document["id"].ToString(),
                Title = result.Document["title"].ToString(),
                Content = result.Document["content"].ToString(),
                Score = result.Score ?? 0,
                Captions = result.Captions?.Select(c => c.Text).ToList(),
                Answers = result.Answers?.Select(a => a.Text).ToList()
            };

            if (result.Document.TryGetValue("metadata", out object metadata))
            {
                searchResult.Metadata = 
                    JsonSerializer.Deserialize<Dictionary<string, string>>(
                        metadata.ToString());
            }

            results.Add(searchResult);
        }

        return results;
    }
}