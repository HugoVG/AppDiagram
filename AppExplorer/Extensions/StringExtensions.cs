namespace AppExplorer.Extensions;

public static class StringExtensions
{
    public static bool ContainsAny(this string str, IEnumerable<string> substrings)
    {
        return substrings.Any(substring => str.Contains(substring));
    }

    public static string ReplaceForbiddenWords(this string str, params string[] forbiddenWords)
    {
        return forbiddenWords.Aggregate(str, (current, forbiddenWord) => current.Replace(forbiddenWord, ""));
    }
}