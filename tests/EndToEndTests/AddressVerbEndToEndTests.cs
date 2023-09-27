namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.AppSettingsJson)]
public class AddressVerbEndToEndTests : BaseEndToEndTests
{
	public AddressVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	public static TheoryData<string[]> FullResponse = new()
	{
		CommandLineArgumentsFakes.AddressBuildCommandLineOptions(TestImagesPathHelper.HasGpsCoordinate.FilePath, AddressListType.FullResponse, ReverseGeocodeProvider.BigDataCloud, BigDataCloudAdminLevelsFakes.Valid())
	};

	[Theory]
	[MemberData(nameof(FullResponse))]
	public async Task Full_Response_Should_Be_Valid_BigDataCloud_Response(string[] args)
	{
		var actualOutput = await RunMain(args);
		var bigDataCloudResponse = JsonSerializer.Deserialize<BigDataCloudResponse>(actualOutput);
		bigDataCloudResponse.Verify();
	}


	public static TheoryData<string[]> SelectedProperties = new()
	{
		CommandLineArgumentsFakes.AddressBuildCommandLineOptions(TestImagesPathHelper.HasGpsCoordinate.FilePath, AddressListType.SelectedProperties, ReverseGeocodeProvider.BigDataCloud, new List<int> { 2, 4, 6, 8 })
	};

	[Theory]
	[MemberData(nameof(SelectedProperties))]
	public async Task Listing_Selected_Properties_Each_Line_Should_Be_Valid_Word(string[] args)
	{
		var actualOutput = await RunMain(args);
		OutputEachLineShouldMatchWithRegex(actualOutput, @"^[\w0-9- ]*$");
	}

	public static TheoryData<string[]> AllProperties = new()
	{
		CommandLineArgumentsFakes.AddressBuildCommandLineOptions(TestImagesPathHelper.HasGpsCoordinate.FilePath, AddressListType.AllAvailableProperties, ReverseGeocodeProvider.BigDataCloud, new List<int> { 2, 4, 6, 8 })
	};

	[Theory]
	[MemberData(nameof(AllProperties))]
	public async Task Listing_All_Properties_Each_Line_Should_Match_With_AdminLevel_And_Its_Value(string[] args)
	{
		var actualOutput = await RunMain(args);
		OutputEachLineShouldMatchWithRegex(actualOutput, @"^AdminLevel\d: [\w0-9- ]*$");
	}

	private static void OutputEachLineShouldMatchWithRegex(string actualOutput, string regex)
	{
		var lines = actualOutput.Split(Environment.NewLine);
		using (new AssertionScope())
		{
			foreach (var line in lines)
				line.Should().MatchRegex(regex);
		}
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
