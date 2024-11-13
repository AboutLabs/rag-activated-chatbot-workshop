using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RagBot.Models
{
    public class BotResponse
    {
        /// <summary>
        /// The generated response text from the bot
        /// </summary>
        [JsonPropertyName("response")]
        public string Response { get; set; }

        /// <summary>
        /// List of source documents used to generate the response
        /// </summary>
        [JsonPropertyName("sources")]
        public List<string> Sources { get; set; } = new List<string>();

        /// <summary>
        /// Confidence score of the generated response (0.0 to 1.0)
        /// </summary>
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        /// <summary>
        /// Timestamp when the response was generated
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Additional metadata about the response
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// List of follow-up questions or suggestions
        /// </summary>
        [JsonPropertyName("suggestions")]
        public List<string> Suggestions { get; set; } = new List<string>();

        /// <summary>
        /// Creates a new instance of BotResponse with required parameters
        /// </summary>
        public BotResponse(string response, double confidence = 1.0)
        {
            Response = response;
            Confidence = confidence;
        }

        /// <summary>
        /// Adds a source document to the response
        /// </summary>
        public void AddSource(string source)
        {
            if (!string.IsNullOrEmpty(source) && !Sources.Contains(source))
            {
                Sources.Add(source);
            }
        }

        /// <summary>
        /// Adds metadata to the response
        /// </summary>
        public void AddMetadata(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                Metadata[key] = value;
            }
        }

        /// <summary>
        /// Adds a follow-up suggestion
        /// </summary>
        public void AddSuggestion(string suggestion)
        {
            if (!string.IsNullOrEmpty(suggestion) && !Suggestions.Contains(suggestion))
            {
                Suggestions.Add(suggestion);
            }
        }

        /// <summary>
        /// Creates a formatted string representation of the response
        /// </summary>
        public override string ToString()
        {
            return $"Response: {Response}\n" +
                   $"Confidence: {Confidence:P}\n" +
                   $"Sources: {Sources.Count}\n" +
                   $"Suggestions: {Suggestions.Count}\n" +
                   $"Generated: {Timestamp:yyyy-MM-dd HH:mm:ss}";
        }
    }
}