using System.Text.RegularExpressions;

namespace PhotoCli.Utils;

public static partial class LogUtilities
{
	[GeneratedRegex(@"\[\d{2}:\d{2}:\d{2}\]\s*")]
	private static partial Regex TimeStampRegex();

	public static IEnumerable<string> RemoveTimeStamps(IReadOnlyList<string> testConsoleLines)
	{
		return testConsoleLines.Select(line => TimeStampRegex().Replace(line, "")).ToList();
	}
}
