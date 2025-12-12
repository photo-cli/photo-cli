using System.IO.Abstractions;
using FluentValidation;

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

	[Theory]
	[InlineData("Default", "Critical")]
	[InlineData("Microsoft", "Warning")]
	[InlineData("System", "Error")]
	public async Task SettingSet_WithANewLogLevelKeyValueShouldPersist_InAppSettingsJson(string logKey, string logValue)
	{
		var expectedSettingsValue = $"{logKey}={logValue}";
		var settingsOptions = new SettingsOptions(nameof(ToolOptionsRaw.LogLevel), expectedSettingsValue);
		var newSavedToolOptionsRawDeserialized = await SetAndGetNewDeserializedAppSettingsJson(settingsOptions);
		newSavedToolOptionsRawDeserialized.Should().NotBeNull();
		var actualSettingsValueSavedOnJson = newSavedToolOptionsRawDeserialized?.LogLevel?[logKey];
		actualSettingsValueSavedOnJson.Should().Be(logValue);
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
		await using var settingsJsonFileStream = fileSystem.FileStream.New("appsettings.json", FileMode.Open);
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
		consoleWriterMock.Setup(s => s.RawWriteLine(It.IsAny<string>()));
		var settingsRunner = new SettingsRunner(settingsOptions, toolOptions, Mock.Of<IFileSystem>(), mockValidator.Object, consoleWriterMock.Object);
		var actualExitCode = await settingsRunner.Execute();
		actualExitCode.Should().Be(ExitCode.Success);
		consoleWriterMock.Verify(v => v.RawWriteLine($"{nameof(ToolOptions.AddressSeparator)}={expectedSettingsValue}"));
	}

	[Fact]
	public async Task Execute_WithoutAnyParameters_ShouldWriteAllToolOptionsToConsole()
	{
		var settingsOptions = new SettingsOptions();
		var toolOptions = ToolOptions.Default();
		var mockValidator = new Mock<IValidator<ToolOptions>>();
		mockValidator.Setup(s => s.Validate(toolOptions)).Returns(ValidationResultFakes.NoError);
		var consoleWriterMock = new Mock<IConsoleWriter>();
		consoleWriterMock.Setup(s => s.RawWriteLine(It.IsAny<string>()));
		var settingsRunner = new SettingsRunner(settingsOptions, toolOptions, Mock.Of<IFileSystem>(), mockValidator.Object, consoleWriterMock.Object);

		var actualExitCode = await settingsRunner.Execute();

		actualExitCode.Should().Be(ExitCode.Success);

		VerifyBasicPropertyOutput(nameof(toolOptions.YearFormat), toolOptions.YearFormat, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.MonthFormat), toolOptions.MonthFormat, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.DayFormat), toolOptions.DayFormat, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.DateFormatWithMonth), toolOptions.DateFormatWithMonth, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.DateFormatWithDay), toolOptions.DateFormatWithDay, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.DateTimeFormatWithMinutes), toolOptions.DateTimeFormatWithMinutes, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.DateTimeFormatWithSeconds), toolOptions.DateTimeFormatWithSeconds, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.AddressSeparator), toolOptions.AddressSeparator, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.FolderAppendSeparator), toolOptions.FolderAppendSeparator, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.DayRangeSeparator), toolOptions.DayRangeSeparator, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.SameNameNumberSeparator), toolOptions.SameNameNumberSeparator, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.PhotoFormatInvalidFolderName), toolOptions.PhotoFormatInvalidFolderName, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.NoPhotoTakenDateFolderName), toolOptions.NoPhotoTakenDateFolderName, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.NoAddressFolderName), toolOptions.NoAddressFolderName, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.NoAddressAndPhotoTakenDateFolderName), toolOptions.NoAddressAndPhotoTakenDateFolderName, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.CsvReportFileName), toolOptions.CsvReportFileName, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.DryRunCsvReportFileName), toolOptions.DryRunCsvReportFileName, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.ConnectionLimit), toolOptions.ConnectionLimit, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.BigDataCloudApiKey), toolOptions.BigDataCloudApiKey, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.GoogleMapsApiKey), toolOptions.GoogleMapsApiKey, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.LocationIqApiKey), toolOptions.LocationIqApiKey, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.CoordinatePrecision), toolOptions.CoordinatePrecision, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.ArchivePhotoTakenDateHashSeparator), toolOptions.ArchivePhotoTakenDateHashSeparator, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.LogCategoryNameOutput), toolOptions.LogCategoryNameOutput, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.MacOsCommand), toolOptions.MacOsCommand, consoleWriterMock);
		VerifyBasicPropertyOutput(nameof(ToolOptions.MacOsArgumentPrefix), toolOptions.MacOsArgumentPrefix, consoleWriterMock);

		VerifyListPropertyOutput(nameof(ToolOptions.SupportedExtensions), toolOptions.SupportedExtensions, consoleWriterMock);
		VerifyListPropertyOutput(nameof(ToolOptions.CompanionExtensions), toolOptions.CompanionExtensions, consoleWriterMock);

		VerifyLogLevelOutputForCategory("Default", consoleWriterMock, toolOptions);
		VerifyLogLevelOutputForCategory("PhotoCli", consoleWriterMock, toolOptions);
		VerifyLogLevelOutputForCategory("PhotoCli.Services.Implementations.ReverseGeocodes", consoleWriterMock, toolOptions);
		VerifyLogLevelOutputForCategory("Polly", consoleWriterMock, toolOptions);
		VerifyLogLevelOutputForCategory("Microsoft", consoleWriterMock, toolOptions);
		VerifyLogLevelOutputForCategory("System.Net.Http.HttpClient", consoleWriterMock, toolOptions);

		consoleWriterMock.VerifyNoOtherCalls();
	}

	private static void VerifyBasicPropertyOutput(string settingsKey, object? settingValue, Mock<IConsoleWriter> consoleWriterMock)
	{
		consoleWriterMock.Verify(v  => v.RawWriteLine($"{settingsKey}={settingValue}"));
	}

	private static void VerifyListPropertyOutput(string settingsKey, string[] settingValues, Mock<IConsoleWriter> consoleWriterMock)
	{
		consoleWriterMock.Verify(v  => v.RawWriteLine($"{settingsKey}={string.Join(",", settingValues)}"));
	}

	private static void VerifyLogLevelOutputForCategory(string logCategoryName, Mock<IConsoleWriter> consoleWriterMock, ToolOptions toolOptions)
	{
		consoleWriterMock.Verify(v  => v.RawWriteLine($"{nameof(ToolOptions.LogLevel)}.{logCategoryName}={toolOptions.LogLevel![logCategoryName]}"));
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
		consoleWriterMock.Verify(v => v.WriteError(errorMessage));
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
