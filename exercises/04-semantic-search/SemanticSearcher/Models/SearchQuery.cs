using System.Text.Json.Serialization;

namespace SemanticSearch.Models
{
    public class SearchQuery
    {
        /// <summary>
        /// The original search text provided by the user
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Vector embedding of the search text
        /// </summary>
        public float[] Embedding { get; set; }

        /// <summary>
        /// Number of results to return
        /// </summary>
        public int ResultCount { get; set; } = 3;

        /// <summary>
        /// Minimum similarity score threshold (0.0 to 1.0)
        /// </summary>
        public double MinScore { get; set; } = 0.7;

        /// <summary>
        /// Language of the search query
        /// </summary>
        public string Language { get; set; } = "en-US";

        /// <summary>
        /// Optional filters to apply to the search
        /// </summary>
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Whether to include semantic captions in the results
        /// </summary>
        public bool IncludeCaptions { get; set; } = true;

        /// <summary>
        /// Whether to include semantic answers in the results
        /// </summary>
        public bool IncludeAnswers { get; set; } = true;

        /// <summary>
        /// Fields to search in
        /// </summary>
        public List<string> SearchFields { get; set; } = new List<string> { "content", "title" };

        /// <summary>
        /// Validates the search query parameters
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return false;

            if (ResultCount < 1 || ResultCount > 50)
                return false;

            if (MinScore < 0.0 || MinScore > 1.0)
                return false;

            if (Embedding != null && Embedding.Length == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Creates a copy of the search query with default values
        /// </summary>
        public static SearchQuery CreateDefault(string searchText)
        {
            return new SearchQuery
            {
                SearchText = searchText,
                ResultCount = 3,
                MinScore = 0.7,
                Language = "en-US",
                IncludeCaptions = true,
                IncludeAnswers = true
            };
        }
    }
}