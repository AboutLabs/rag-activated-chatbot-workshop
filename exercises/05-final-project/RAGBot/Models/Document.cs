public class Document
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string Source { get; set; }
    public DateTime ProcessedDate { get; set; }
}

public class SearchResult
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string Source { get; set; }
    public double Score { get; set; }
}

public class BotResponse
{
    public string Response { get; set; }
    public List<string> Sources { get; set; }
}