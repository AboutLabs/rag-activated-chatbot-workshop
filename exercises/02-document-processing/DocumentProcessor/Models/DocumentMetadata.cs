public class DocumentMetadata
{
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime CreationDate { get; set; }
    public string DocumentType { get; set; }
    public string Language { get; set; }
    public string Source { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
    public Dictionary<string, string> AdditionalMetadata { get; set; } = new Dictionary<string, string>();

    public DocumentMetadata()
    {
        CreationDate = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
    }

    public void AddMetadata(string key, string value)
    {
        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        {
            AdditionalMetadata[key] = value;
        }
    }

    public string GetMetadata(string key)
    {
        return AdditionalMetadata.TryGetValue(key, out string value) ? value : null;
    }

    public override string ToString()
    {
        return $"Document: {Title}\n" +
               $"Author: {Author}\n" +
               $"Created: {CreationDate:yyyy-MM-dd HH:mm:ss}\n" +
               $"Type: {DocumentType}\n" +
               $"Language: {Language}\n" +
               $"Source: {Source}";
    }
}