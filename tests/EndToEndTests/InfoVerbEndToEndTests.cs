namespace PhotoCli.Tests.EndToEndTests;

public class InfoVerbEndToEndTests : BaseEndToEndTests
{
	private const string ReportFileName = "report.csv";
	private readonly FileInfo _reportFile = new(ReportFileName);

	public static TheoryData<string[], List<PhotoCsv>, Statistics> SingleFolderOnlyPhotoTakenDate = new()
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
			StatisticsFakes.Basic(18, hasTakenDateAndCoordinate: 15, hasNoTakenDateAndCoordinate: 2, hasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, Statistics> SingleFolderPhotoTakenDateAndAddress = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SingleFolder(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false, ReverseGeocodeProvider.BigDataCloud, ["3", "4", "5", "6", "7"]),
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
			StatisticsFakes.Basic(18, hasTakenDateAndCoordinate: 15, hasNoTakenDateAndCoordinate: 2, hasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, Statistics> SubFoldersOnlyPhotoTakenDate = new()
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
			StatisticsFakes.Basic(18, hasTakenDateAndCoordinate: 15, hasNoTakenDateAndCoordinate: 2, hasTakenDateButNoCoordinate: 1)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, Statistics> SubFoldersPhotoTakenDateAndAddress = new()
	{
		{
			CommandLineArgumentsFakes.InfoBuildCommandLineOptions(ReportFileName, TestImagesPathHelper.SubFolders(), true, InfoNoPhotoTakenDateAction.Continue,
				InfoNoCoordinateAction.Continue, false, ReverseGeocodeProvider.BigDataCloud, ["3", "4", "5", "6", "7"]),
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
			StatisticsFakes.Basic(18, hasTakenDateAndCoordinate: 15, hasNoTakenDateAndCoordinate: 2, hasTakenDateButNoCoordinate: 1)
		}
	};

	[Theory]
	[MemberData(nameof(SingleFolderOnlyPhotoTakenDate))]
	[MemberData(nameof(SingleFolderPhotoTakenDateAndAddress))]
	[MemberData(nameof(SubFoldersOnlyPhotoTakenDate))]
	[MemberData(nameof(SubFoldersPhotoTakenDateAndAddress))]
	public async Task Running_InfoVerbArguments_ShouldCreateAndVerifyPhotosAndReportCsvOnFileSystem(string[] args, List<PhotoCsv> expectedPhotoCsvModels, Statistics expectedStatistics)
	{
		CleanArtifacts();
		var actualStatistics = await RunMainOutputAsStatistics(args);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(_reportFile);
		using (new AssertionScope())
		{
			actualStatistics.Should().BeEquivalentTo(expectedStatistics, c => c
				.Excluding(e => e.ReserveGeocodeFromMemory)
				.Excluding(e => e.ReserveGeocodeRequestSent)
			);

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
