public class SearchResult
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public double Score { get; set; }
    public List<string> Captions { get; set; }
    public List<string> Answers { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}

public class SearchQuery
{
    public string SearchText { get; set; }
    public int ResultCount { get; set; }
    public float[] Embedding { get; set; }
}