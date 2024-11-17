public class DocumentChunk
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string SourceDocument { get; set; }
    public int PageNumber { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
} 