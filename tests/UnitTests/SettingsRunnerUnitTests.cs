using System.IO.Abstractions;
using FluentValidation;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests;

public class SettingsRunnerUnitTests
{
	#region Expected Code Flow

	[Fact]
	public async Task Setting_A_New_Value_Should_Persist_In_AppSettings_Json()
	{
		const string expectedSettingsValue = "test-value";
		var settingsOptions = new SettingsOptions(nameof(ToolOptionsRaw.AddressSeparator), expectedSettingsValue);

		var newSavedToolOptionsRawDeserialized = await SetAndGetNewDeserializedAppSettingsJson(settingsOptions);

		var actualSettingsValueSavedOnJson = newSavedToolOptionsRawDeserialized?.AddressSeparator;
		actualSettingsValueSavedOnJson.Should().Be(expectedSettingsValue);
	}

	[Fact]
	public async Task Setting_A_New_Int_Value_Should_Persist_In_AppSettings_Json()
	{
		const int expectedSettingsValue = 7;
		var settingsOptions = new SettingsOptions(nameof(ToolOptionsRaw.CoordinatePrecision), expectedSettingsValue.ToString());

		var newSavedToolOptionsRawDeserialized = await SetAndGetNewDeserializedAppSettingsJson(settingsOptions);

		var actualSettingsValueSavedOnJson = newSavedToolOptionsRawDeserialized?.CoordinatePrecision;
		actualSettingsValueSavedOnJson.Should().Be(expectedSettingsValue);
	}

	[Fact]
	public async Task Setting_A_New_LogLevel_Should_Persist_In_AppSettings_Json()
	{
		var expectedSettingsValue = LogLevel.Critical.ToString();
		var settingsOptions = new SettingsOptions(nameof(ToolOptionsRaw.LogLevel), expectedSettingsValue);
		var newSavedToolOptionsRawDeserialized = await SetAndGetNewDeserializedAppSettingsJson(settingsOptions);
		var actualSettingsValueSavedOnJson = newSavedToolOptionsRawDeserialized?.LogLevel?.Default;
		actualSettingsValueSavedOnJson.Should().Be(expectedSettingsValue);
	}

	private static async Task<ToolOptionsRaw?> SetAndGetNewDeserializedAppSettingsJson(SettingsOptions settingsOptions, ToolOptions? toolOptions = null)
	{
		toolOptions ??= ToolOptions.Default();
		var fileSystem = new MockFileSystem();
		var validatorMock = new Mock<IValidator<ToolOptions>>();
		validatorMock.Setup(s => s.Validate(It.IsAny<ToolOptions>())).Returns(ValidationResultFakes.NoError);
		var settingsRunner = new SettingsRunner(settingsOptions, toolOptions, fileSystem, validatorMock.Object, Mock.Of<IConsoleWriter>());
		var actualExitCode = await settingsRunner.Execute();
		actualExitCode.Should().Be(ExitCode.Success);
		await using var settingsJsonFileStream = fileSystem.FileStream.Create("appsettings.json", FileMode.Open);
		var newSavedToolOptionsRawDeserialized = await JsonSerializer.DeserializeAsync<ToolOptionsRaw>(settingsJsonFileStream);
		return newSavedToolOptionsRawDeserialized;
	}

	[Fact]
	public async Task Getting_A_Value_Should_Be_Written_To_Console()
	{
		const string expectedSettingsValue = "test-value";
		var settingsOptions = new SettingsOptions(nameof(ToolOptionsRaw.AddressSeparator));
		var toolOptions = ToolOptions.Default();
		toolOptions.AddressSeparator = expectedSettingsValue;
		var mockValidator = new Mock<IValidator<ToolOptions>>();
		mockValidator.Setup(s => s.Validate(toolOptions)).Returns(ValidationResultFakes.NoError);
		var consoleWriterMock = new Mock<IConsoleWriter>();
		consoleWriterMock.Setup(s => s.Write(It.IsAny<string>()));
		var settingsRunner = new SettingsRunner(settingsOptions, toolOptions, Mock.Of<IFileSystem>(), mockValidator.Object, consoleWriterMock.Object);
		var actualExitCode = await settingsRunner.Execute();
		actualExitCode.Should().Be(ExitCode.Success);
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.AddressSeparator)}={expectedSettingsValue}"));
	}

	[Fact]
	public async Task Getting_All_Values_Should_Be_Written_To_Console()
	{
		var settingsOptions = new SettingsOptions();
		var toolOptions = ToolOptions.Default();
		toolOptions.AddressSeparator = "address-separator";
		toolOptions.ConnectionLimit = 100;
		toolOptions.CoordinatePrecision = 7;
		toolOptions.DayFormat = "day-format";
		toolOptions.LogLevel = new Options.LogLevel { Default = "Critical" };
		toolOptions.MonthFormat = "month-format";
		toolOptions.YearFormat = "year-format";
		toolOptions.DayRangeSeparator = "day-range-separator";
		toolOptions.SameNameNumberSeparator = "same-name-number-separator";
		toolOptions.FolderAppendSeparator = "folder-append-separator";
		toolOptions.CsvReportFileName = "csv-report-file-name";
		toolOptions.DateFormatWithDay = "date-format-with-day";
		toolOptions.DateFormatWithMonth = "date-format-with-month";
		toolOptions.BigDataCloudApiKey = "big-data-cloud-api-key";
		toolOptions.GoogleMapsApiKey = "google-maps-api-key";
		toolOptions.LocationIqApiKey = "location-iq-api-key";
		toolOptions.NoAddressFolderName = "no-address-folder-name";
		toolOptions.DateTimeFormatWithMinutes = "date-time-format-with-minutes";
		toolOptions.DateTimeFormatWithSeconds = "date-time-format-with-seconds";
		toolOptions.DryRunCsvReportFileName = "dry-run-csv-report-file-name";
		toolOptions.NoPhotoTakenDateFolderName = "no-photo-taken-date-folder-name";
		toolOptions.NoAddressAndPhotoTakenDateFolderName = "no-address-and-photo-taken-date-folder-name";
		toolOptions.ArchivePhotoTakenDateHashSeparator = "archive-photo-taken-date-hash-separator";

		var mockValidator = new Mock<IValidator<ToolOptions>>();
		mockValidator.Setup(s => s.Validate(toolOptions)).Returns(ValidationResultFakes.NoError);
		var consoleWriterMock = new Mock<IConsoleWriter>();
		consoleWriterMock.Setup(s => s.Write(It.IsAny<string>()));
		var settingsRunner = new SettingsRunner(settingsOptions, toolOptions, Mock.Of<IFileSystem>(), mockValidator.Object, consoleWriterMock.Object);

		var actualExitCode = await settingsRunner.Execute();

		actualExitCode.Should().Be(ExitCode.Success);
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.AddressSeparator)}={toolOptions.AddressSeparator}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.ConnectionLimit)}={toolOptions.ConnectionLimit.ToString()}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.CoordinatePrecision)}={toolOptions.CoordinatePrecision.ToString()}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.DayFormat)}={toolOptions.DayFormat}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.LogLevel)}={toolOptions.LogLevel.Default}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.MonthFormat)}={toolOptions.MonthFormat}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.YearFormat)}={toolOptions.YearFormat}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.DayRangeSeparator)}={toolOptions.DayRangeSeparator}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.SameNameNumberSeparator)}={toolOptions.SameNameNumberSeparator}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.FolderAppendSeparator)}={toolOptions.FolderAppendSeparator}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.CsvReportFileName)}={toolOptions.CsvReportFileName}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.DateFormatWithDay)}={toolOptions.DateFormatWithDay}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.DateFormatWithMonth)}={toolOptions.DateFormatWithMonth}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.BigDataCloudApiKey)}={toolOptions.BigDataCloudApiKey}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.GoogleMapsApiKey)}={toolOptions.GoogleMapsApiKey}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.LocationIqApiKey)}={toolOptions.LocationIqApiKey}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.NoAddressFolderName)}={toolOptions.NoAddressFolderName}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.DateTimeFormatWithMinutes)}={toolOptions.DateTimeFormatWithMinutes}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.DateTimeFormatWithSeconds)}={toolOptions.DateTimeFormatWithSeconds}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.DryRunCsvReportFileName)}={toolOptions.DryRunCsvReportFileName}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.NoPhotoTakenDateFolderName)}={toolOptions.NoPhotoTakenDateFolderName}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.NoAddressAndPhotoTakenDateFolderName)}={toolOptions.NoAddressAndPhotoTakenDateFolderName}"));
		consoleWriterMock.Verify(v => v.Write($"{nameof(ToolOptions.ArchivePhotoTakenDateHashSeparator)}={toolOptions.ArchivePhotoTakenDateHashSeparator}"));
	}

	[Fact]
	public async Task Resetting_Should_Persist_Default_Values_In_AppSettings_Json()
	{
		var changedToolOptions = ToolOptions.Default();
		changedToolOptions.AddressSeparator = "changed";
		var settingsOptions = new SettingsOptions(reset: true);
		var newSavedToolOptionsRawDeserialized = await SetAndGetNewDeserializedAppSettingsJson(settingsOptions, changedToolOptions);
		newSavedToolOptionsRawDeserialized.Should().NotBeNull();
		var actualToolOptions = new ToolOptions(newSavedToolOptionsRawDeserialized!);
		actualToolOptions.AddressSeparator.Should().Be("-").And.NotBe("changed");
	}

	#endregion

	#region Breaking Code Flow

	[Fact]
	public async Task Setting_An_Invalid_Value_Should_Write_Errors_To_Console_And_Return_With_ExitCode_ValidationFailed()
	{
		var validatorMock = new Mock<IValidator<ToolOptions>>();
		var consoleWriterMock = new Mock<IConsoleWriter>();
		const string errorMessage = "Validation error message";
		validatorMock.Setup(s => s.Validate(It.IsAny<ToolOptions>())).Returns(() => ValidationResultFakes.HasErrors(errorMessage));
		var settingsRunner = new SettingsRunner(SettingsOptionsFakes.Set(), ToolOptions.Default(), Mock.Of<IFileSystem>(), validatorMock.Object, consoleWriterMock.Object);
		var actualExitCode = await settingsRunner.Execute();
		actualExitCode.Should().Be(ExitCode.InvalidSettingsValue);
		consoleWriterMock.Verify(v => v.Write(errorMessage));
		consoleWriterMock.VerifyNoOtherCalls();
	}


	[Fact]
	public async Task Getting_A_Invalid_Property_Should_Return_With_ExitCode_PropertyNotFound()
	{
		var settingsOptions = new SettingsOptions("invalid-key");
		var settingsRunner = new SettingsRunner(settingsOptions, ToolOptions.Default(), Mock.Of<IFileSystem>(), Mock.Of<IValidator<ToolOptions>>(), Mock.Of<IConsoleWriter>());
		var actualExitCode = await settingsRunner.Execute();
		actualExitCode.Should().Be(ExitCode.PropertyNotFound);
	}

	[Fact]
	public async Task Setting_A_Invalid_Property_Should_Return_With_ExitCode_PropertyNotFound()
	{
		var settingsOptions = new SettingsOptions("invalid-key", "value");
		var settingsRunner = new SettingsRunner(settingsOptions, ToolOptions.Default(), Mock.Of<IFileSystem>(), Mock.Of<IValidator<ToolOptions>>(), Mock.Of<IConsoleWriter>());
		var actualExitCode = await settingsRunner.Execute();
		actualExitCode.Should().Be(ExitCode.PropertyNotFound);
	}

	#endregion
}
