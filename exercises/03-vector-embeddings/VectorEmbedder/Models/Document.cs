using System.Text.Json.Serialization;

public class Document
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("vector")]
    public float[] Vector { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; }
}

public class SearchResult
{
    public string Id { get; set; }
    public string Content { get; set; }
    public double Score { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}