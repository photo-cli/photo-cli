namespace PhotoCli.Tests.EndToEndTests;

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
			actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels, c => c
				.Excluding(e => e.ReverseGeocodeFormatted)
				.Excluding(e => e.Address1).Excluding(e => e.Address2).Excluding(e => e.Address3).Excluding(e => e.Address4)
				.Excluding(e => e.Address5).Excluding(e => e.Address6).Excluding(e => e.Address7).Excluding(e => e.Address8));
		}
		CleanArtifacts();
	}

	private void CleanArtifacts()
	{
		if (_reportFile.Exists)
			_reportFile.Delete();
	}
}
