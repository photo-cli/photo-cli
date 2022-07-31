using FluentValidation;

namespace PhotoCli.Options.Validators;

public class ToolOptionsValidator : AbstractValidator<ToolOptions>
{
	public ToolOptionsValidator()
	{
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
		RuleFor(r => r.NoPhotoTakenDateFolderName).RequiredString();
		RuleFor(r => r.NoAddressFolderName).RequiredString();
		RuleFor(r => r.NoAddressAndPhotoTakenDateFolderName).RequiredString();

		RuleFor(r => r.CsvReportFileName).RequiredString().Matches(Constants.CsvExtensionRegex);
		RuleFor(r => r.DryRunCsvReportFileName).RequiredString().Matches(Constants.CsvExtensionRegex);

		RuleFor(r => r.LogLevel.Default).Must(m => Enum.TryParse(typeof(Microsoft.Extensions.Logging.LogLevel), m, out _)).WithMessage(LogLevels());
	}

	private string LogLevels()
	{
		var levels = string.Join(", ", Enum.GetNames<Microsoft.Extensions.Logging.LogLevel>());
		return $"Log level should be on of these values: {levels}";
	}

	private bool DateTimeFormatIsValid(string newFormat)
	{
		var value = DateTime.Now.ToString(newFormat);
		var hasAnyAffect = value != newFormat;
		return hasAnyAffect;
	}

	private string DateTimeFormatMessage(string property, string example)
	{
		return $"{property} is not date format. Valid example is {example}";
	}
}
