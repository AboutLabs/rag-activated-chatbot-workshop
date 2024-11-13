public class ProcessedDocument
{
    public string Id { get; set; }
    public string Content { get; set; }
    public DocumentMetadata Metadata { get; set; }
    public DateTime ProcessedDate { get; set; }
}

public class DocumentMetadata
{
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime CreationDate { get; set; }
    public string DocumentType { get; set; }
    public string Language { get; set; }
}