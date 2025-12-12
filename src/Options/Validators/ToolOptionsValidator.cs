using FluentValidation;
using Microsoft.Extensions.Logging;

namespace PhotoCli.Options.Validators;

public class ToolOptionsValidator : AbstractValidator<ToolOptions>
{
	private readonly ILogger<ToolOptionsValidator> _logger;

	public ToolOptionsValidator(ILogger<ToolOptionsValidator> logger)
	{
		_logger = logger;
		RuleFor(r => r.YearFormat).Must(DateTimeFormatIsValid).WithMessage(DateTimeFormatMessage(nameof(ToolOptions.YearFormat), ToolOptions.YearFormatDefault));
		RuleFor(r => r.MonthFormat).Must(DateTimeFormatIsValid).WithMessage(DateTimeFormatMessage(nameof(ToolOptions.MonthFormat), ToolOptions.MonthFormatDefault));
		RuleFor(r => r.DayFormat).Must(DateTimeFormatIsValid).WithMessage(DateTimeFormatMessage(nameof(ToolOptions.DayFormat), ToolOptions.DayFormatDefault));
		RuleFor(r => r.DateFormatWithMonth).Must(DateTimeFormatIsValid).WithMessage(DateTimeFormatMessage(nameof(ToolOptions.DateFormatWithMonth), ToolOptions.DateFormatWithMonthDefault));
		RuleFor(r => r.DateFormatWithDay).Must(DateTimeFormatIsValid).WithMessage(DateTimeFormatMessage(nameof(ToolOptions.DateFormatWithDay), ToolOptions.DateFormatWithDayDefault));
		RuleFor(r => r.DateTimeFormatWithMinutes).Must(DateTimeFormatIsValid)
			.WithMessage(DateTimeFormatMessage(nameof(ToolOptions.DateTimeFormatWithMinutes), ToolOptions.DateTimeFormatWithMinutesDefault));
		RuleFor(r => r.DateTimeFormatWithSeconds).Must(DateTimeFormatIsValid)
			.WithMessage(DateTimeFormatMessage(nameof(ToolOptions.DateTimeFormatWithSeconds), ToolOptions.DateTimeFormatWithSecondsDefault));

		RuleFor(r => r.AddressSeparator).RequiredString();
		RuleFor(r => r.FolderAppendSeparator).RequiredString();
		RuleFor(r => r.DayRangeSeparator).RequiredString();
		RuleFor(r => r.SameNameNumberSeparator).RequiredString();
		RuleFor(r => r.PhotoFormatInvalidFolderName).RequiredString();
		RuleFor(r => r.NoPhotoTakenDateFolderName).RequiredString();
		RuleFor(r => r.NoAddressFolderName).RequiredString();
		RuleFor(r => r.NoAddressAndPhotoTakenDateFolderName).RequiredString();

		RuleFor(r => r.CsvReportFileName).RequiredString().Matches(Constants.CsvExtensionRegex);
		RuleFor(r => r.DryRunCsvReportFileName).RequiredString().Matches(Constants.CsvExtensionRegex);

		RuleFor(r => r.LogLevel).Must(ValidateLogLevels).WithMessage("One or more log levels are invalid. Log level should be one of these values: " + LogLevels());
	}

	private bool ValidateLogLevels(Dictionary<string, string>? logLevels)
	{
		if (logLevels == null)
			return true;

		foreach (var (logCategoryNamespaceValue, logLevel) in logLevels)
		{
			if (Enum.TryParse<Microsoft.Extensions.Logging.LogLevel>(logLevel, true, out _))
				continue;
			_logger.LogError("Invalid log level '{LogLevel}' for category '{Category}'", logLevel, logCategoryNamespaceValue);
			return false;
		}

		return true;
	}

	private static string LogLevels()
	{
		return string.Join(", ", Enum.GetNames<Microsoft.Extensions.Logging.LogLevel>());
	}

	private bool DateTimeFormatIsValid(string newFormat)
	{
		try
		{
			var value = DateTime.Now.ToString(newFormat);
			var hasAnyAffect = value != newFormat;
			return hasAnyAffect;
		}
		catch (FormatException formatException)
		{
			_logger.LogInformation(formatException, "DateTime format exception for {Format}", newFormat);
			return false;
		}
	}

	private string DateTimeFormatMessage(string property, string example)
	{
		return $"{property} is not date format. Valid example is {example}";
	}
}
