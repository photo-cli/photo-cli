using Microsoft.Extensions.Configuration;

namespace PhotoCli.Options;

public class LogLevel
{
	public string? Default { get; set; }
	public Dictionary<string, string> _categories = new();

	public string? GetLogLevel(string categoryName)
	{
		if (_categories.TryGetValue(categoryName, out var level))
			return level;
		return Default;
	}
}
