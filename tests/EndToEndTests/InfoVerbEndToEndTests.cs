namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.EndToEndTests)]
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
			[
				SingleKenya(),
				SingleItalyFlorence(),
				SingleNoGpsCoordinate(),
				SingleItalyArezzo1(),
				SingleItalyArezzo2(),
				SingleItalyArezzo3(),
				SingleItalyArezzo4(),
				SingleItalyArezzo5(),
				SingleItalyArezzo6(),
				SingleItalyArezzo7(),
				SingleItalyArezzo8(),
				SingleItalyArezzo9(),
				SingleItalyArezzo9Duplicate(),
				SingleUnitedKingdom(),
				SingleSpain1(),
				SingleSpain2(),
				SingleNoGpsCoordinateAndNoPhotoTakenDate(),
				SingleNoPhotoTakenDate()
			],
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderPhotoTakenDateAndAddress = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SingleFolder(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false, ReverseGeocodeProvider.BigDataCloud, new List<string> { "3", "4", "5", "6", "7" }),
			[
				SingleKenya(),
				SingleItalyFlorence(),
				SingleNoGpsCoordinate(),
				SingleItalyArezzo1(),
				SingleItalyArezzo2(),
				SingleItalyArezzo3(),
				SingleItalyArezzo4(),
				SingleItalyArezzo5(),
				SingleItalyArezzo6(),
				SingleItalyArezzo7(),
				SingleItalyArezzo8(),
				SingleItalyArezzo9(),
				SingleItalyArezzo9Duplicate(),
				SingleUnitedKingdom(),
				SingleSpain1(),
				SingleSpain2(),
				SingleNoGpsCoordinateAndNoPhotoTakenDate(),
				SingleNoPhotoTakenDate()
			],
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersOnlyPhotoTakenDate = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SubFolders(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false),
			[
				SubFoldersKenya(),
				SubFoldersItalyFlorence(),
				SubFoldersNoGpsCoordinate(),
				SubFoldersItalyArezzo1(),
				SubFoldersItalyArezzo2(),
				SubFoldersItalyArezzo3(),
				SubFoldersItalyArezzo4(),
				SubFoldersItalyArezzo5(),
				SubFoldersItalyArezzo6(),
				SubFoldersItalyArezzo7(),
				SubFoldersItalyArezzo8(),
				SubFoldersItalyArezzo9(),
				SubFoldersItalyArezzo9Duplicate(),
				SubFoldersUnitedKingdom(),
				SubFoldersSpain1(),
				SubFoldersSpain2(),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate(),
				SubFoldersNoPhotoTakenDate()
			],
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPhotoTakenDateAndAddress = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SubFolders(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false, ReverseGeocodeProvider.BigDataCloud, new List<string> { "3", "4", "5", "6", "7" }),
			[
				SubFoldersKenya(),
				SubFoldersItalyFlorence(),
				SubFoldersNoGpsCoordinate(),
				SubFoldersItalyArezzo1(),
				SubFoldersItalyArezzo2(),
				SubFoldersItalyArezzo3(),
				SubFoldersItalyArezzo4(),
				SubFoldersItalyArezzo5(),
				SubFoldersItalyArezzo6(),
				SubFoldersItalyArezzo7(),
				SubFoldersItalyArezzo8(),
				SubFoldersItalyArezzo9(),
				SubFoldersItalyArezzo9Duplicate(),
				SubFoldersUnitedKingdom(),
				SubFoldersSpain1(),
				SubFoldersSpain2(),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate(),
				SubFoldersNoPhotoTakenDate()
			],
			new ConsoleOutputValues(18, HasTakenDateAndCoordinate: 15, HasNoTakenDateAndCoordinate: 2, HasTakenDateButNoCoordinate: 1)
		}
	};

	[Theory]
	[MemberData(nameof(SingleFolderOnlyPhotoTakenDate))]
	[MemberData(nameof(SingleFolderPhotoTakenDateAndAddress))]
	[MemberData(nameof(SubFoldersOnlyPhotoTakenDate))]
	[MemberData(nameof(SubFoldersPhotoTakenDateAndAddress))]
	public async Task Running_InfoVerbArguments_ShouldCreateAndVerifyPhotosAndReportCsvOnFileSystem(string[] args, List<PhotoCsv> expectedPhotoCsvModels,
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
