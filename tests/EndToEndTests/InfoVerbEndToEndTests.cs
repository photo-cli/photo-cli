namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.AppSettingsJson)]
public class InfoVerbEndToEndTests : BaseEndToEndTests
{
	public InfoVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	private const string ReportFileName = "report.csv";
	private readonly FileInfo _reportFile = new(ReportFileName);

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderOnlyPhotoTakenDate = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SingleFolder(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false),
			new List<PhotoCsv>
			{
				SingleKenya(useFullPath: false),
				SingleItalyFlorence(useFullPath: false),
				SingleNoGpsCoordinate(useFullPath: false),
				SingleItalyArezzo1(useFullPath: false),
				SingleItalyArezzo2(useFullPath: false),
				SingleItalyArezzo3(useFullPath: false),
				SingleItalyArezzo4(useFullPath: false),
				SingleItalyArezzo5(useFullPath: false),
				SingleItalyArezzo6(useFullPath: false),
				SingleItalyArezzo7(useFullPath: false),
				SingleItalyArezzo8(useFullPath: false),
				SingleItalyArezzo9(useFullPath: false),
				SingleItalyArezzo9Duplicate(useFullPath: false),
				SingleUnitedKingdom(useFullPath: false),
				SingleSpain1(useFullPath: false),
				SingleSpain2(useFullPath: false),
				SingleNoGpsCoordinateAndNoPhotoTakenDate(useFullPath: false),
				SingleNoPhotoTakenDate(useFullPath: false),
			},
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderPhotoTakenDateAndAddress = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SingleFolder(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false, ReverseGeocodeProvider.BigDataCloud, new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya(null, false, "Nakuru County"),
				SingleItalyFlorence(null, false, "Tuscany", "Metropolitan City of Florence"),
				SingleNoGpsCoordinate(useFullPath: false),
				SingleItalyArezzo1(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo2(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo3(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo4(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo5(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo6(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo7(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo8(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo9(null, false, "Tuscany", "Province of Arezzo"),
				SingleItalyArezzo9Duplicate(null, false, "Tuscany", "Province of Arezzo"),
				SingleUnitedKingdom(null, false, "England", "South East England", "Windsor and Maidenhead"),
				SingleSpain1(null, false, "Community of Madrid", "Community of Madrid", "Madrid Metropolitan Area"),
				SingleSpain2(null, false, "Community of Madrid", "Community of Madrid", "Madrid Metropolitan Area"),
				SingleNoGpsCoordinateAndNoPhotoTakenDate(useFullPath: false),
				SingleNoPhotoTakenDate(useFullPath: false),
			},
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersOnlyPhotoTakenDate = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SubFolders(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false),
			new List<PhotoCsv>
			{
				SubFoldersKenya(useFullPath: false),
				SubFoldersItalyFlorence(useFullPath: false),
				SubFoldersNoGpsCoordinate(useFullPath: false),
				SubFoldersItalyArezzo1(useFullPath: false),
				SubFoldersItalyArezzo2(useFullPath: false),
				SubFoldersItalyArezzo3(useFullPath: false),
				SubFoldersItalyArezzo4(useFullPath: false),
				SubFoldersItalyArezzo5(useFullPath: false),
				SubFoldersItalyArezzo6(useFullPath: false),
				SubFoldersItalyArezzo7(useFullPath: false),
				SubFoldersItalyArezzo8(useFullPath: false),
				SubFoldersItalyArezzo9(useFullPath: false),
				SubFoldersItalyArezzo9Duplicate(useFullPath: false),
				SubFoldersUnitedKingdom(useFullPath: false),
				SubFoldersSpain1(useFullPath: false),
				SubFoldersSpain2(useFullPath: false),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate(useFullPath: false),
				SubFoldersNoPhotoTakenDate(useFullPath: false),
			},
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPhotoTakenDateAndAddress = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SubFolders(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false, ReverseGeocodeProvider.BigDataCloud, new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(null, false, "Nakuru County"),
				SubFoldersItalyFlorence(null, false, "Tuscany", "Metropolitan City of Florence"),
				SubFoldersNoGpsCoordinate(useFullPath: false),
				SubFoldersItalyArezzo1(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo2(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo3(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo4(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo5(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo6(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo7(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo8(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo9(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate(null, false, "Tuscany", "Province of Arezzo"),
				SubFoldersUnitedKingdom(null, false, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersSpain1(null, false, "Community of Madrid", "Community of Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2(null, false, "Community of Madrid", "Community of Madrid", "Madrid Metropolitan Area"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate(useFullPath: false),
				SubFoldersNoPhotoTakenDate(useFullPath: false),
			},
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	[Theory]
	[MemberData(nameof(SingleFolderOnlyPhotoTakenDate))]
	[MemberData(nameof(SingleFolderPhotoTakenDateAndAddress))]
	[MemberData(nameof(SubFoldersOnlyPhotoTakenDate))]
	[MemberData(nameof(SubFoldersPhotoTakenDateAndAddress))]
	public async Task Running_With_Info_Verb_Arguments_Should_Create_And_Verify_Photos_And_Report_Csv_On_File_System(string[] args, List<PhotoCsv> expectedPhotoCsvModels,
		ConsoleOutputValues expectedConsoleOutput)
	{
		CleanArtifacts();
		var actualOutput = await RunMain(args);
		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(_reportFile);
		using (new AssertionScope())
		{
			actualConsoleOutput.Should().Be(expectedConsoleOutput);
			actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
		}

		CleanArtifacts();
	}

	private void CleanArtifacts()
	{
		if (_reportFile.Exists)
			_reportFile.Delete();
	}
}
