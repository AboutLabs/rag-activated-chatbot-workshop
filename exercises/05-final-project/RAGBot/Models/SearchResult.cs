using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RagBot.Models
{
    public class SearchResult
    {
        /// <summary>
        /// Unique identifier of the search result
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Title or heading of the found document
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Main content or text of the search result
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// Relevance score of the search result (0.0 to 1.0)
        /// </summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }

        /// <summary>
        /// Source of the document (e.g., file path, URL)
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// Date when the document was last modified
        /// </summary>
        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Extracted key phrases or important terms
        /// </summary>
        [JsonPropertyName("keyPhrases")]
        public List<string> KeyPhrases { get; set; } = new List<string>();

        /// <summary>
        /// Document type (PDF, Word, HTML, etc.)
        /// </summary>
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        /// <summary>
        /// Additional metadata about the search result
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Vector representation of the content
        /// </summary>
        [JsonPropertyName("contentVector")]
        public float[] ContentVector { get; set; }

        /// <summary>
        /// Highlighted snippets from the content
        /// </summary>
        [JsonPropertyName("highlights")]
        public List<string> Highlights { get; set; } = new List<string>();

        /// <summary>
        /// Creates a formatted string representation of the search result
        /// </summary>
        public override string ToString()
        {
            return $"Title: {Title}\n" +
                   $"Score: {Score:F2}\n" +
                   $"Source: {Source}\n" +
                   $"Type: {DocumentType}\n" +
                   $"Modified: {LastModified:yyyy-MM-dd HH:mm:ss}";
        }

        /// <summary>
        /// Adds a key phrase to the result
        /// </summary>
        public void AddKeyPhrase(string keyPhrase)
        {
            if (!string.IsNullOrEmpty(keyPhrase) && !KeyPhrases.Contains(keyPhrase))
            {
                KeyPhrases.Add(keyPhrase);
            }
        }

        /// <summary>
        /// Adds metadata to the result
        /// </summary>
        public void AddMetadata(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                Metadata[key] = value;
            }
        }
    }
}