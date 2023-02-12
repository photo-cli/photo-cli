namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.AppSettingsJson)]
public class SettingsVerbEndToEndTests : BaseEndToEndTests
{
	public SettingsVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	private const string AppSettingsJsonFileName = "appsettings.json";

	private const string DefaultAppSettingsValue = @"{
    ""LogLevel"": {
      ""Default"": ""Error""
    },
    ""YearFormat"": ""yyyy"",
    ""MonthFormat"": ""MM"",
    ""DayFormat"": ""dd"",
    ""DateFormatWithMonth"": ""yyyy.MM"",
    ""DateFormatWithDay"": ""yyyy.MM.dd"",
    ""DateTimeFormatWithMinutes"": ""yyyy.MM.dd_HH.mm"",
    ""DateTimeFormatWithSeconds"": ""yyyy.MM.dd_HH.mm.ss"",
    ""AddressSeparator"": ""-"",
    ""FolderAppendSeparator"": ""-"",
    ""DayRangeSeparator"": ""-"",
	""SameNameNumberSeparator"": ""-"",
    ""NoPhotoTakenDateFolderName"": ""no-photo-taken-date"",
    ""NoAddressFolderName"": ""no-address"",
    ""NoAddressAndPhotoTakenDateFolderName"": ""no-address-and-no-photo-taken-date"",
    ""CsvReportFileName"": ""photo-cli-report.csv"",
    ""DryRunCsvReportFileName"": ""photo-cli-dry-run.csv"",
    ""ConnectionLimit"": 4,
    ""BigDataCloudApiKey"": """",
    ""GoogleMapsApiKey"": """",
    ""LocationIqApiKey"": """"
}";

	public static TheoryData<string[], string> List = new()
	{
		{
			CommandLineArgumentsFakes.SettingsBuildCommandLineOptions(),
			@"LogLevel=Error
YearFormat=yyyy
MonthFormat=MM
DayFormat=dd
DateFormatWithMonth=yyyy.MM
DateFormatWithDay=yyyy.MM.dd
DateTimeFormatWithMinutes=yyyy.MM.dd_HH.mm
DateTimeFormatWithSeconds=yyyy.MM.dd_HH.mm.ss
AddressSeparator=-
FolderAppendSeparator=-
DayRangeSeparator=-
SameNameNumberSeparator=-
NoPhotoTakenDateFolderName=no-photo-taken-date
NoAddressFolderName=no-address
NoAddressAndPhotoTakenDateFolderName=no-address-and-no-photo-taken-date
CsvReportFileName=photo-cli-report.csv
DryRunCsvReportFileName=photo-cli-dry-run.csv
ConnectionLimit=4
BigDataCloudApiKey=
GoogleMapsApiKey=
LocationIqApiKey="
		}
	};

	[Theory]
	[MemberData(nameof(List))]
	public async Task Running_With_Settings_Verb_Arguments_With_Listing_Should_Be_Match_With_The_Values_Of_AppSettings_json_File(string[] args, string expectedOutput)
	{
		ResetAppSettings();
		var actualOutput = await RunMain(args);
		StringsShouldMatchDiscardingLineEndings(actualOutput, expectedOutput);
		var actualAppSettingsValues = ActualAppSettingsJson();
		var expectedAppSettingsValues = DefaultAppSettingsJson();
		actualAppSettingsValues.Should().BeEquivalentTo(expectedAppSettingsValues);
		ResetAppSettings();
	}

	#region Get Value

	public static TheoryData<string[], string, string> GetValue = new()
	{
		{
			CommandLineArgumentsFakes.SettingsBuildCommandLineOptions("YearFormat"),
			"YearFormat",
			@"yyyy"
		}
	};

	[Theory]
	[MemberData(nameof(GetValue))]
	public async Task Running_With_Settings_Verb_Arguments_With_A_Get_Value_Should_Be_Match_With_Expected_Output(string[] args, string key, string expectedValue)
	{
		ResetAppSettings();
		var actualOutput = await RunMain(args);
		var expectedOutput = $"{key}={expectedValue}";
		actualOutput.Should().Be(expectedOutput);
		var appSettingsValues = ActualAppSettingsJson();
		var actualValue = appSettingsValues[key];
		actualValue.Should().Be(expectedValue);
		ResetAppSettings();
	}

	#endregion

	#region Set Value

	public static TheoryData<string[], string, string> SetValue = new()
	{
		{
			CommandLineArgumentsFakes.SettingsBuildCommandLineOptions("YearFormat", "y"),
			"YearFormat",
			"y"
		}
	};

	[Theory]
	[MemberData(nameof(SetValue))]
	public async Task Running_With_Settings_Verb_Arguments_With_A_Set_Value_Should_Write_To_AppSettings_json_File(string[] args, string key, string expectedValue)
	{
		ResetAppSettings();
		var actualOutput = await RunMain(args);
		actualOutput.Should().Be(string.Empty);
		var appSettingsValues = ActualAppSettingsJson();
		var actualValue = appSettingsValues[key];
		actualValue.Should().Be(expectedValue);
		ResetAppSettings();
	}

	#endregion

	#region Shared

	private Dictionary<string, string> ActualAppSettingsJson()
	{
		var settingsValue = File.ReadAllText(AppSettingsJsonFileName);
		return DeserializeAppSettingsJson(settingsValue);
	}

	private Dictionary<string, string> DefaultAppSettingsJson()
	{
		return DeserializeAppSettingsJson(DefaultAppSettingsValue);
	}

	private static Dictionary<string, string> DeserializeAppSettingsJson(string settingsValue)
	{
		var appSettingsElements = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(settingsValue);
		if (appSettingsElements == null)
			throw new Exception($"Can't deserialize {AppSettingsJsonFileName}");

		var appSettingsValues = new Dictionary<string, string>();
		foreach (var (propertyName, jsonElement) in appSettingsElements)
		{
			if (jsonElement.ValueKind == JsonValueKind.String)
			{
				appSettingsValues.Add(propertyName, jsonElement.GetString()!);
			}
			else if (jsonElement.ValueKind == JsonValueKind.Number)
			{
				appSettingsValues.Add(propertyName, jsonElement.GetInt32().ToString());
			}
			else if (propertyName == "LogLevel")
			{
				var logLevelElement = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonElement.GetRawText());
				if (logLevelElement == null)
					continue;
				appSettingsValues.Add(propertyName, logLevelElement.Single()!.Value.GetString()!);
			}
		}

		return appSettingsValues;
	}

	private void ResetAppSettings()
	{
		File.WriteAllText(AppSettingsJsonFileName, DefaultAppSettingsValue);
	}

	#endregion
}
