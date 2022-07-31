using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class ToolOptionsFluentValidationTests : BaseFluentValidationTests<ToolOptions, ToolOptionsValidator>
{
	#region Valid

	[Fact]
	public void Default_ToolOptions_Should_Have_No_Error()
	{
		var toolOptions = ToolOptions.Default();
		ValidationShouldHaveNoError(toolOptions);
	}

	[Fact]
	public void Setting_Valid_LogLevel_Should_Have_No_Error()
	{
		var toolOptions = ToolOptions.Default();
		toolOptions.LogLevel.Default = LogLevel.Critical.ToString();
		ValidationShouldHaveNoError(toolOptions);
	}

	#endregion

	#region Invalid

	#region DateTime Formats

	[Fact]
	public void YearFormat_Invalid_Should_Give_PredicateValidator_Error_And_Verify_Error_Message()
	{
		Should_Give_PredicateValidator_Error_And_Verify_Error_Message(new ToolOptionsRaw { YearFormat = "abc" }, nameof(ToolOptions.YearFormat), "yyyy");
	}

	[Fact]
	public void MonthFormat_Invalid_Should_Give_PredicateValidator_Error_And_Verify_Error_Message()
	{
		Should_Give_PredicateValidator_Error_And_Verify_Error_Message(new ToolOptionsRaw { MonthFormat = "abc" }, nameof(ToolOptions.MonthFormat), "MM");
	}

	[Fact]
	public void DayFormat_Invalid_Should_Give_PredicateValidator_Error_And_Verify_Error_Message()
	{
		Should_Give_PredicateValidator_Error_And_Verify_Error_Message(new ToolOptionsRaw { DayFormat = "abc" }, nameof(ToolOptions.DayFormat), "dd");
	}

	[Fact]
	public void DateFormatWithMonth_Invalid_Should_Give_PredicateValidator_Error_And_Verify_Error_Message()
	{
		Should_Give_PredicateValidator_Error_And_Verify_Error_Message(new ToolOptionsRaw { DateFormatWithMonth = "abc" }, nameof(ToolOptions.DateFormatWithMonth), "yyyy.MM");
	}

	[Fact]
	public void DateFormatWithDay_Invalid_Should_Give_PredicateValidator_Error_And_Verify_Error_Message()
	{
		Should_Give_PredicateValidator_Error_And_Verify_Error_Message(new ToolOptionsRaw { DateFormatWithDay = "abc" }, nameof(ToolOptions.DateFormatWithDay), "yyyy.MM.dd");
	}

	[Fact]
	public void DateTimeFormatWithMinutes_Invalid_Should_Give_PredicateValidator_Error_And_Verify_Error_Message()
	{
		Should_Give_PredicateValidator_Error_And_Verify_Error_Message(new ToolOptionsRaw { DateTimeFormatWithMinutes = "abc" }, nameof(ToolOptions.DateTimeFormatWithMinutes), "yyyy.MM.dd_HH.mm");
	}

	[Fact]
	public void DateTimeFormatWithSeconds_Invalid_Should_Give_PredicateValidator_Error_And_Verify_Error_Message()
	{
		Should_Give_PredicateValidator_Error_And_Verify_Error_Message(new ToolOptionsRaw { DateTimeFormatWithSeconds = "abc" }, nameof(ToolOptions.DateTimeFormatWithSeconds),
			"yyyy.MM.dd_HH.mm.ss");
	}

	private void Should_Give_PredicateValidator_Error_And_Verify_Error_Message(ToolOptionsRaw optionsRaw, string property, string example)
	{
		var options = new ToolOptions(optionsRaw);
		var errorMessage = DateTimeFormatMessage(property, example);
		CheckPropertyInvalidValue(options, property, errorMessage);
	}

	#endregion

	#region Required String

	[Theory]
	[InlineData(nameof(ToolOptions.AddressSeparator))]
	[InlineData(nameof(ToolOptions.FolderAppendSeparator))]
	[InlineData(nameof(ToolOptions.DayRangeSeparator))]
	[InlineData(nameof(ToolOptions.SameNameNumberSeparator))]
	[InlineData(nameof(ToolOptions.NoPhotoTakenDateFolderName))]
	[InlineData(nameof(ToolOptions.NoAddressFolderName))]
	[InlineData(nameof(ToolOptions.NoAddressAndPhotoTakenDateFolderName))]
	[InlineData(nameof(ToolOptions.CsvReportFileName))]
	[InlineData(nameof(ToolOptions.DryRunCsvReportFileName))]
	public void Given_Properties_Should_Give_RequiredStringValidator_Error_And_Verify_Error_Message(string property)
	{
		DynamicallyCheckRequiredStringInvalidValue(property);
	}

	private void DynamicallyCheckRequiredStringInvalidValue(string property)
	{
		DynamicallyCheckRequiredStringInvalidValue(property, string.Empty);
		DynamicallyCheckRequiredStringInvalidValue(property, null);
	}

	private void DynamicallyCheckRequiredStringInvalidValue(string property, string? value)
	{
		var toolOptions = ToolOptions.Default();
		var propertyInfo = typeof(ToolOptions).GetProperty(property);
		propertyInfo!.SetValue(toolOptions, value);
		CheckPropertyRequiredString(toolOptions, property);
	}

	#endregion

	[Fact]
	public void Setting_Invalid_LogLevel_Should_Give_PredicateValidator_And_Verify_Error_Message()
	{
		var toolOptions = ToolOptions.Default();
		toolOptions.LogLevel.Default = "invalid-log-level";
		CheckPropertyInvalidValue(toolOptions, $"{nameof(ToolOptions.LogLevel)}.{nameof(ToolOptions.LogLevel.Default)}", LogLevels());
	}

	private string LogLevels()
	{
		var levels = string.Join(", ", Enum.GetNames<LogLevel>());
		return $"Log level should be on of these values: {levels}";
	}

	#endregion

	private string DateTimeFormatMessage(string property, string example)
	{
		return $"{property} is not date format. Valid example is {example}";
	}
}
