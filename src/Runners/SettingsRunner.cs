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
			if (_cliOptions.Key == nameof(LogLevel))
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
				var propertyValue = Convert.ChangeType(_cliOptions.Value, property.PropertyType);
				property.SetValue(_toolOptions, propertyValue);
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
			ConsoleWriteProperty(property);
		}
		else if (_cliOptions.Reset)
		{
			var defaultOptions = ToolOptions.Default();
			await PersistToSettingsFile(defaultOptions);
		}
		else
		{
			foreach (var property in GetProperties())
				ConsoleWriteProperty(property);
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

	private IEnumerable<PropertyInfo> GetProperties()
	{
		return typeof(ToolOptions).GetProperties().OrderBy(o => o.Name);
	}

	private void ConsoleWriteProperty(PropertyInfo property)
	{
		if (property.Name == nameof(LogLevel))
			ConsoleWriteKeyValue(property.Name, _toolOptions.LogLevel.Default);
		else
			ConsoleWriteKeyValue(property.Name, property.GetValue(_toolOptions)?.ToString());
	}

	private void ConsoleWriteKeyValue(string key, string? value)
	{
		_consoleWriter.Write($"{key}={value}");
	}
}
