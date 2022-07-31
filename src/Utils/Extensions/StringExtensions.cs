namespace PhotoCli.Utils.Extensions;

public static class StringExtensions
{
	public static string RemoveFirst(this string text, string search)
	{
		return ReplaceFirst(text, search, string.Empty);
	}

	public static string ReplaceFirst(this string text, string search, string replace)
	{
		var index = text.IndexOf(search, StringComparison.Ordinal);
		if (index < 0)
			return text;
		return text[..index] + replace + text[(index + search.Length)..];
	}

	public static bool IsPresent(this string? value)
	{
		return !string.IsNullOrEmpty(value);
	}

	public static bool IsMissing(this string? value)
	{
		return string.IsNullOrEmpty(value);
	}
}
