namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.AppSettingsJson)]
public class AddressVerbEndToEndTests : BaseEndToEndTests
{
	public AddressVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	public static TheoryData<string[], string> SelectedProperties = new()
	{
		{
			CommandLineArgumentsFakes.AddressBuildCommandLineOptions(TestImagesPathHelper.HasGpsCoordinate.FilePath, AddressListType.SelectedProperties, ReverseGeocodeProvider.BigDataCloud,
				new List<int> { 2, 4, 6, 8 }),
			@"Italy
Toscana
Province of Arezzo
Arezzo"
		}
	};

	public static TheoryData<string[], string> AllProperties = new()
	{
		{
			CommandLineArgumentsFakes.AddressBuildCommandLineOptions(TestImagesPathHelper.HasGpsCoordinate.FilePath, AddressListType.AllAvailableProperties, ReverseGeocodeProvider.BigDataCloud, new List<int> { 2, 4, 6, 8 }),
			@"AdminLevel2: Italy
AdminLevel4: Toscana
AdminLevel6: Province of Arezzo
AdminLevel8: Arezzo"
		}
	};

	public static TheoryData<string[], string> FullResponse = new()
	{
		{
			CommandLineArgumentsFakes.AddressBuildCommandLineOptions(TestImagesPathHelper.HasGpsCoordinate.FilePath, AddressListType.FullResponse, ReverseGeocodeProvider.BigDataCloud, BigDataCloudAdminLevelsFakes.Valid()),
			@"{
""localityInfo"": {
""administrative"": [
{
""adminLevel"": 2,
""isoName"": null,
""name"": ""Italy"",
""isoCode"": ""IT"",
""description"": ""republic in Southern Europe"",
""order"": 2,
""wikidataId"": ""Q38"",
""geoNameId"": null
},
{
""adminLevel"": 4,
""isoName"": null,
""name"": ""Toscana"",
""isoCode"": ""IT-52"",
""description"": ""region in central Italy"",
""order"": 4,
""wikidataId"": ""Q1273"",
""geoNameId"": null
},
{
""adminLevel"": 6,
""isoName"": null,
""name"": ""Province of Arezzo"",
""isoCode"": ""IT-AR"",
""description"": ""province of Italy"",
""order"": 5,
""wikidataId"": ""Q16115"",
""geoNameId"": null
},
{
""adminLevel"": 8,
""isoName"": null,
""name"": ""Arezzo"",
""isoCode"": null,
""description"": ""Italian comune"",
""order"": 6,
""wikidataId"": ""Q13378"",
""geoNameId"": null
}
],
""informative"": [
{
""isoCode"": ""EU"",
""name"": ""Europe"",
""order"": 1,
""description"": ""continent"",
""wikidataId"": ""Q46"",
""geonameId"": 6255148
},
{
""isoCode"": null,
""name"": ""Italian Peninsula"",
""order"": 3,
""description"": ""peninsula of southern Europe"",
""wikidataId"": ""Q145694"",
""geonameId"": null
}
]
},
""latitude"": 43.46744833333334,
""longitude"": 11.885126666663888,
""plusCode"": ""8FMHFV8P\u002BX3"",
""localityLanguageRequested"": ""en"",
""continent"": ""Europe"",
""continentCode"": ""EU"",
""countryName"": ""Italy"",
""countryCode"": ""IT"",
""principalSubdivision"": ""Toscana"",
""city"": ""Arezzo"",
""locality"": ""Arezzo"",
""postcode"": """"
}"
		}
	};

	[Theory]
	[MemberData(nameof(FullResponse))]
	[MemberData(nameof(SelectedProperties))]
	[MemberData(nameof(AllProperties))]
	public async Task Running_With_Address_Verb_Arguments_Should_Be_Match_With_Expected_Output(string[] args, string expectedOutput)
	{
		var actualOutput = await RunMain(args);
		StringsShouldMatchDiscardingLineEndings(actualOutput, expectedOutput);
	}

	public static TheoryData<string[], string> NotExistingImage = new()
	{
		{
			CommandLineArgumentsFakes.AddressBuildCommandLineOptions("file-doesnt-exits.jpg", AddressListType.AllAvailableProperties, ReverseGeocodeProvider.BigDataCloud, BigDataCloudAdminLevelsFakes.Valid()),
			"Process failed with a error code 26 (InputFileNotExists)"
		}
	};

	[Theory]
	[MemberData(nameof(NotExistingImage))]
	public async Task NotExistingImage_Should_Exit_With_InputFileNotExists_And_Output_Should_Match_With_Expected(string[] args, string expectedOutput)
	{
		var actualOutput = await RunMain(args, ExitCode.InputFileNotExists);
		StringsShouldMatchDiscardingLineEndings(actualOutput, expectedOutput);
	}
}
