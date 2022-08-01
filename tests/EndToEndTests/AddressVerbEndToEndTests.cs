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
Tuscany
Province of Arezzo
Arezzo"
		}
	};

	public static TheoryData<string[], string> AllProperties = new()
	{
		{
			CommandLineArgumentsFakes.AddressBuildCommandLineOptions(TestImagesPathHelper.HasGpsCoordinate.FilePath, AddressListType.AllAvailableProperties, ReverseGeocodeProvider.BigDataCloud, new List<int> { 2, 4, 6, 8 }),
			@"AdminLevel2: Italy
AdminLevel4: Tuscany
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
""isoName"": ""Italy"",
""name"": ""Italy"",
""isoCode"": ""IT"",
""description"": ""republic in Southern Europe"",
""order"": 3,
""wikidataId"": ""Q38"",
""geoNameId"": null
},
{
""adminLevel"": 4,
""isoName"": ""Toscana"",
""name"": ""Tuscany"",
""isoCode"": ""IT-52"",
""description"": ""region in central Italy with an area of about 23,000 square kilometres (8,900 square miles). The regional capital is Florence"",
""order"": 5,
""wikidataId"": ""Q1273"",
""geoNameId"": null
},
{
""adminLevel"": 6,
""isoName"": ""Arezzo"",
""name"": ""Province of Arezzo"",
""isoCode"": ""IT-AR"",
""description"": ""province of Italy"",
""order"": 6,
""wikidataId"": ""Q16115"",
""geoNameId"": null
},
{
""adminLevel"": 8,
""isoName"": null,
""name"": ""Arezzo"",
""isoCode"": null,
""description"": ""Italian comune"",
""order"": 7,
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
""name"": ""Geographical region of Italy"",
""order"": 2,
""description"": ""geographical region"",
""wikidataId"": ""Q3155864"",
""geonameId"": null
},
{
""isoCode"": null,
""name"": ""Italian Peninsula"",
""order"": 4,
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
""principalSubdivision"": ""Tuscany"",
""city"": """",
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
