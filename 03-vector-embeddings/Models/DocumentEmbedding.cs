public class DocumentEmbedding
{
    public string Id { get; set; }
    public string Content { get; set; }
    public float[] Embedding { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
} 