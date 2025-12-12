using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Utils.Logging;

public static class LoggerExtensions
{
	public static void LogErrorWithPath(this ILogger logger, string message, string filePath, params object?[] args)
	{
		LogWithPath(logger, LogLevel.Error, message, filePath, args);
	}

	public static void LogWarningWithPath(this ILogger logger, string message, string filePath)
	{
		LogWithPath(logger, LogLevel.Warning, message, filePath);
	}
	private static void LogWithPath(ILogger logger, LogLevel logLevel, string message, string filePath, params object[] args)
	{
		List<object>? newArgs;
		if (args.Length == 0)
			newArgs = [filePath];
		else
			newArgs = [.. args, filePath];
#pragma warning disable CA2254
		// ReSharper disable once TemplateIsNotCompileTimeConstantProblem
		logger.Log(logLevel, message + $". Path:<{{{AnsiConsoleLogger.PathVariable}}}>", newArgs.ToArray());
#pragma warning restore CA2254
	}

	public static void LogWithTwoPaths(this ILogger logger, LogLevel logLevel, string message, string filePath1, string filePath2)
	{
#pragma warning disable CA2254
		// ReSharper disable once TemplateIsNotCompileTimeConstantProblem
		logger.Log(logLevel, message + $": <{{{AnsiConsoleLogger.Path1Variable}}}>, <{{{AnsiConsoleLogger.Path2Variable}}}>", filePath1, filePath2);
#pragma warning restore CA2254
	}
}
