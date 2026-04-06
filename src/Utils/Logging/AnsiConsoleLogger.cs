using System.Collections.Concurrent;
using Spectre.Console;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Utils.Logging;

public class AnsiConsoleLogger : ILogger
{
	public const string PathVariable = "Path";
	public const string Path1Variable = "Path1";
	public const string Path2Variable = "Path2";
	private const string LoggerOriginalFormat = "{OriginalFormat}";

	private readonly AnsiConsoleExtended _ansiConsoleExtended;
	private readonly string _categoryName;
	private readonly LogLevel _defaultLogLevel;
	private readonly ConcurrentDictionary<string, LogLevel> _logLevels;
	private readonly ConcurrentDictionary<string, LogLevel> _logLevelCacheByNamespace;
	private readonly bool _logCategoryNameOutput;

	private static readonly Dictionary<string, string> OverrideSpectreFormatByLogParameterKey = new()
	{
		{ PathVariable, "italic grey" },
		{ Path1Variable, "italic grey" },
		{ Path2Variable, "italic grey" },
	};

	public AnsiConsoleLogger(string categoryName, LogLevel defaultLogLevel, ConcurrentDictionary<string, LogLevel> logLevels, ConcurrentDictionary<string, LogLevel> logLevelCacheByNamespace,
		bool logCategoryNameOutput, AnsiConsoleExtended ansiConsoleExtended)
	{
		_ansiConsoleExtended = ansiConsoleExtended;
		_categoryName = categoryName;
		_defaultLogLevel = defaultLogLevel;
		_logLevels = logLevels;
		_logLevelCacheByNamespace = logLevelCacheByNamespace;
		_logCategoryNameOutput = logCategoryNameOutput;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
			return;

		string? logContent = null;
		var spectreFormat = logLevel switch
		{
			LogLevel.Trace => "grey",
			LogLevel.Debug => "blue",
			LogLevel.Information => "green",
			LogLevel.Warning => "yellow",
			LogLevel.Error => "red",
			LogLevel.Critical => "red bold",
			_ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Unknown log level")
		};

		if (state is IEnumerable<KeyValuePair<string, object?>> logItems)
		{
			var parameterByKey = logItems.ToDictionary(k => k.Key, v => v.Value);
			if (parameterByKey.TryGetValue(LoggerOriginalFormat, out var originalFormatValue) && originalFormatValue is string originalFormatString)
			{
				logContent = originalFormatString;
				var otherParameters = parameterByKey.Where(k => k.Key != LoggerOriginalFormat);
				foreach (var (key, value) in otherParameters)
				{
					if (value is null)
						continue;

					bool isJson = false;
					string? valueFormat;
					if (value is string || value.GetType().IsPrimitive || value.GetType().IsEnum)
						valueFormat = value.ToString();
					else if (value is IEnumerable<string> stringEnumerable)
						valueFormat = string.Join(", ", stringEnumerable);
					else if (value is IEnumerable<KeyValuePair<string, string>> stringKeyValuePairs)
						valueFormat = string.Join(", ", stringKeyValuePairs.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
					else if (value is IEnumerable<KeyValuePair<int, string>> intKeyValuePairs)
						valueFormat = string.Join(", ", intKeyValuePairs.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
					else
					{
						valueFormat = JsonSerializer.Serialize(value, StaticOptions.AnsiConsoleLoggerOptions);
						isJson = true;
					}

					if (valueFormat == null)
						throw new PhotoCliException($"valueFormat value is null. Key: {key}");

					valueFormat = _ansiConsoleExtended.EscapeMarkup(valueFormat);

					if (OverrideSpectreFormatByLogParameterKey.TryGetValue(key, out var overrideColor))
						valueFormat = _ansiConsoleExtended.OutputTextByFormat(valueFormat, overrideColor);

					if (isJson)
					{
						valueFormat = _ansiConsoleExtended.OutputTextByFormat(valueFormat, Color.Aqua);
						valueFormat = _ansiConsoleExtended.OutputBetweenNewLines(valueFormat);
					}

					logContent = logContent.Replace($"{{{key}}}", valueFormat);
				}
			}
		}
		logContent ??= formatter(state, exception);

		if (_logCategoryNameOutput)
			logContent += $" on ({_ansiConsoleExtended.OutputTextByFormat(_categoryName, Color.Grey)})";

		if (exception != null)
			logContent += Environment.NewLine + _ansiConsoleExtended.EscapeMarkup(exception.ToString());

		_ansiConsoleExtended.WriteLineWithTime(logContent, spectreFormat);
	}
	public bool IsEnabled(LogLevel logLevel)
	{
		return logLevel >= LogLevelOfCategory();
	}

	private LogLevel LogLevelOfCategory()
	{
		if (_logLevelCacheByNamespace.TryGetValue(_categoryName, out var cachedLogLevel))
			return cachedLogLevel;

		LogLevel? level = null;
		foreach (var namespaceHierarchyCategoryName in GetNamespaceHierarchy(_categoryName))
		{
			if (!_logLevels.TryGetValue(namespaceHierarchyCategoryName, out var logLevel))
				continue;
			level = logLevel;
			break;
		}
		var categoryLevel = level ?? _defaultLogLevel;
		_logLevelCacheByNamespace.TryAdd(_categoryName, categoryLevel);
		return categoryLevel;
	}

	private static List<string> GetNamespaceHierarchy(string fullNamespace)
	{
		var parts = fullNamespace.Split('.');
		var result = new List<string>();
		for (var i = parts.Length; i > 0; i--)
		{
			var namespacePart = string.Join(".", parts.Take(i));
			result.Add(namespacePart);
		}
		return result;
	}

	public IDisposable BeginScope<TState>(TState state) where TState : notnull
	{
		return null!;
	}
}
