using System.Collections.Generic;

public class RagBotState
{
    public List<DocumentEmbedding> LoadedDocuments { get; set; } = new List<DocumentEmbedding>();
    public string CurrentMode { get; set; } = "chat"; // "chat" or "document"
    public int TurnCount { get; set; }
} 