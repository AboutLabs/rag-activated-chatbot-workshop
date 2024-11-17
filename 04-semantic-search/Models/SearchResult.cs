public class SearchResult
{
    public string Id { get; set; }
    public string Content { get; set; }
    public double Score { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
} 