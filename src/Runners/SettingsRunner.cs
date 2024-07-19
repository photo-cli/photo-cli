using System.IO.Abstractions;
using FluentValidation;
using LogLevel = PhotoCli.Options.LogLevel;

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
				_toolOptions.LogLevel = new LogLevel
				{
					Default = _cliOptions.Value
				};
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
				else if(property.PropertyType.BaseType == typeof(ValueType) || property.PropertyType == typeof(string))
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
		}
		else if (_cliOptions.Key != null)
		{
			var property = GetPropertyByKey();
			if (property == null)
				return ExitCode.PropertyNotFound;
			if (property.PropertyType.BaseType == typeof(Array))
				ConsoleWriteArrayProperty(property);
			else
				ConsoleWriteBasicProperty(property);
		}
		else if (_cliOptions.Reset)
		{
			var defaultOptions = ToolOptions.Default();
			await PersistToSettingsFile(defaultOptions);
		}
		else
		{
			ConsoleWriteKeyValue(nameof(ToolOptions.LogLevel), _toolOptions.LogLevel.Default);
			foreach (var property in GetArrayProperties())
				ConsoleWriteArrayProperty(property);
			foreach (var property in GetBasicProperties())
				ConsoleWriteBasicProperty(property);
		}

		return ExitCode.Success;
	}

	private bool Validate(ToolOptions options)
	{
		var validationResult = _toolOptionsValidator.Validate(options);
		if (!validationResult.IsValid)
		{
			foreach (var validationResultError in validationResult.Errors)
				_consoleWriter.Write(validationResultError.ErrorMessage);
			return false;
		}

		return true;
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

	private IEnumerable<PropertyInfo> GetBasicProperties()
	{
		return typeof(ToolOptions).GetProperties()
			.Where(w => w.PropertyType.BaseType == typeof(ValueType) || w.PropertyType == typeof(string))
			.OrderBy(o => o.Name);
	}

	private IEnumerable<PropertyInfo> GetArrayProperties()
	{
		return typeof(ToolOptions).GetProperties()
			.Where(x => x.PropertyType.BaseType == typeof(Array))
			.OrderBy(o => o.Name);
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

	private void ConsoleWriteKeyValue(string key, string? value)
	{
		_consoleWriter.Write($"{key}={value}");
	}
}
