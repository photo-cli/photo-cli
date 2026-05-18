using System.IO.Abstractions;
using FluentValidation;

namespace PhotoCli.Runners;

public class SettingsRunner : IConsoleRunner
{
	private readonly SettingsOptions _cliOptions;
	private readonly IConsoleWriter _consoleWriter;
	private readonly IFileSystem _fileSystem;
	private readonly ToolOptions _toolOptions;
	private readonly IValidator<ToolOptions> _toolOptionsValidator;

	public SettingsRunner(SettingsOptions cliOptions, ToolOptions toolOptions, IFileSystem fileSystem, IValidator<ToolOptions> toolOptionsValidator, IConsoleWriter consoleWriter)
	{
		_cliOptions = cliOptions;
		_fileSystem = fileSystem;
		_toolOptionsValidator = toolOptionsValidator;
		_consoleWriter = consoleWriter;
		_toolOptions = toolOptions;
	}

	public async Task<ExitCode> Execute()
	{
		if (_cliOptions.Key != null && _cliOptions.Value != null)
		{
			if (_cliOptions.Key == nameof(ToolOptions.LogLevel))
			{
				if (_toolOptions.LogLevel == null)
					throw new PhotoCliException("LogLevel dictionary should be already initialized or at least as with default values");

				var valuesSplitByEqualSign = _cliOptions.Value.Split("=").Select(s => s.Trim()).ToArray();
				if (valuesSplitByEqualSign.Length != 2)
				{
					_consoleWriter.WriteError("LogLevel setting requires a value in the format of Namespace=LogLevel, e.g. Default=Information , System.Net.Http.HttpClient=Error");
					return ExitCode.InvalidSettingsLogLevelChange;
				}
				var namespaceValue = valuesSplitByEqualSign[0];
				var logLevelRaw = valuesSplitByEqualSign[1];

				_toolOptions.LogLevel[namespaceValue] = logLevelRaw;
			}
			else
			{
				var property = GetPropertyByKey();
				if (property == null)
					return ExitCode.PropertyNotFound;
				if (property.PropertyType == typeof(string[]))
				{
					var values = _cliOptions.Value.Split(",").Select(s => s.Trim()).ToArray();
					property.SetValue(_toolOptions, values);
				}
				else if (property.PropertyType.BaseType == typeof(ValueType) || property.PropertyType == typeof(string))
				{
					var propertyValue = Convert.ChangeType(_cliOptions.Value, property.PropertyType);
					property.SetValue(_toolOptions, propertyValue);
				}
				else
				{
					throw new PhotoCliException("Not defined setting type to set");
				}
			}

			if (!Validate(_toolOptions))
				return ExitCode.InvalidSettingsValue;
			await PersistToSettingsFile(_toolOptions);
			_consoleWriter.WriteSuccess("Settings have been saved");
		}
		else if (_cliOptions.Key != null)
		{
			var property = GetPropertyByKey();
			if (property == null)
				return ExitCode.PropertyNotFound;
			if (property.PropertyType.BaseType == typeof(Array))
				ConsoleWriteArrayProperty(property);
			else if (property.PropertyType == typeof(Dictionary<string, string>))
				ConsoleWriteStringDictionary(property, false);
			else
				ConsoleWriteBasicProperty(property);
		}
		else if (_cliOptions.Reset)
		{
			var defaultOptions = ToolOptions.Default();
			await PersistToSettingsFile(defaultOptions);
			_consoleWriter.WriteSuccess("Settings have been reset");
		}
		else
		{
			foreach (var property in typeof(ToolOptions).GetProperties())
			{
				if (property.PropertyType.BaseType == typeof(Array))
					ConsoleWriteArrayProperty(property);
				else if (property.PropertyType == typeof(Dictionary<string, string>))
					ConsoleWriteStringDictionary(property, true);
				else
					ConsoleWriteBasicProperty(property);
			}

			/*foreach (var property in GetArrayProperties())
				ConsoleWriteArrayProperty(property);
			foreach (var property in GetBasicProperties())
				ConsoleWriteBasicProperty(property);*/
		}

		return ExitCode.Success;
	}

	private bool Validate(ToolOptions options)
	{
		var validationResult = _toolOptionsValidator.Validate(options);
		if (validationResult.IsValid)
			return true;
		foreach (var validationResultError in validationResult.Errors)
			_consoleWriter.WriteError(validationResultError.ToString());
		return false;
	}

	private async Task PersistToSettingsFile(ToolOptions options)
	{
		string appSettingsFilePath;
		if (Environment.CommandLine.Contains(".store"))
		{
			var runningDll = _fileSystem.FileInfo.New(Environment.CommandLine);
			appSettingsFilePath = Path.Combine(runningDll.Directory!.FullName, Constants.AppSettingsFileName);
		}
		else
		{
			appSettingsFilePath = Constants.AppSettingsFileName;
		}

		await using var stream = _fileSystem.FileStream.New(appSettingsFilePath, FileMode.Create);
		await JsonSerializer.SerializeAsync(stream, options, new JsonSerializerOptions { WriteIndented = true });
	}

	private PropertyInfo? GetPropertyByKey()
	{
		return _cliOptions.Key != null ? typeof(ToolOptions).GetProperty(_cliOptions.Key) : null;
	}

	private void ConsoleWriteBasicProperty(PropertyInfo property)
	{
		ConsoleWriteKeyValue(property.Name, property.GetValue(_toolOptions)?.ToString());
	}

	private void ConsoleWriteArrayProperty(PropertyInfo property)
	{
		var arrayValue = (object[]?)property.GetValue(_toolOptions);
		var values = arrayValue != null ? string.Join(",", arrayValue) : string.Empty;
		ConsoleWriteKeyValue(property.Name, values);
	}

	private void ConsoleWriteStringDictionary(PropertyInfo property, bool outputWithDictionaryName)
	{
		if (property.GetValue(_toolOptions) is not Dictionary<string, string> dictionary)
			throw new PhotoCliException($"{property.Name} is not a string dictionary");
		foreach (var (key, value) in dictionary)
		{
			var keyToOutput = outputWithDictionaryName ? $"{property.Name}.{key}" : key;
			ConsoleWriteKeyValue(keyToOutput, value);
		}
	}

	private void ConsoleWriteKeyValue(string key, string? value)
	{
		_consoleWriter.RawWriteLine($"{key}={value}");
	}
}
