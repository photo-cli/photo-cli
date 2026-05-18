using System.Collections.Concurrent;
using Spectre.Console;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Utils.Logging;

public class AnsiConsoleLoggerProvider : ILoggerProvider
{
	private readonly AnsiConsoleExtended _ansiConsoleExtended;
	private readonly LogLevel _defaultLogLevel;
	private readonly ConcurrentDictionary<string, LogLevel> _logLevelConfiguration = new();
	private readonly ConcurrentDictionary<string, LogLevel> _logLevelCacheByNamespace = new();
	private readonly bool _logCategoryNameOutput;

	public AnsiConsoleLoggerProvider(ToolOptions toolOptions, IConsoleWriter consoleWriter, AnsiConsoleExtended ansiConsoleExtended)
	{
		_ansiConsoleExtended = ansiConsoleExtended;
		if (toolOptions.LogLevel == null)
		{
			_defaultLogLevel = LogLevel.Warning;
			return;
		}
		foreach (var (namespaceValue, logLevelRaw) in toolOptions.LogLevel)
		{
			if (!Enum.TryParse<LogLevel>(logLevelRaw, out var logLevelConfiguration))
			{
				consoleWriter.WriteError($"Invalid log level value of {logLevelRaw} for namespace {namespaceValue}, discarding");
				continue;
			}
			if (namespaceValue == "Default")
			{
				_defaultLogLevel = logLevelConfiguration;
				continue;
			}
			_logLevelConfiguration[namespaceValue] = logLevelConfiguration;
		}
		_logCategoryNameOutput = toolOptions.LogCategoryNameOutput;
	}

	public ILogger CreateLogger(string categoryName)
	{
		return new AnsiConsoleLogger(categoryName, _defaultLogLevel, _logLevelConfiguration, _logLevelCacheByNamespace, _logCategoryNameOutput, _ansiConsoleExtended);
	}

	public void Dispose()
	{
	}
}
