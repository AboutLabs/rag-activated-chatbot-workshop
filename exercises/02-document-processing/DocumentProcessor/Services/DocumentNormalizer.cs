public class DocumentNormalizer
{
    public string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\t", " ")
            .Replace("  ", " ")
            .Trim();
    }

    public string RemoveSpecialCharacters(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return Regex.Replace(text, @"[^a-zA-Z0-9\s]", " ");
    }
}