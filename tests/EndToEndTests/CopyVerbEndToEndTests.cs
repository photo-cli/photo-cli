namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.AppSettingsJson)]
public class CopyVerbEndToEndTests : BaseEndToEndTests
{
	private const string OutputPath = "output-folder";


	private readonly FileInfo _csvReportFile = new(Path.Combine(OutputPath, ToolOptionFakes.CsvReportFileName));
	private readonly DirectoryInfo _outputDirectory = new(OutputPath);
	private readonly FileInfo _dryRunCsvReportFile = new(ToolOptionFakes.DryRunCsvReportFileName);

	public CopyVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	#region Single FolderFlattenAllSubFoldersWithDayNamingUsingPaddingZeroCharacterAllNamesAreSameLength

	#region Only Photo Taken Date

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithNumericNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("1"),
				SingleItalyFlorence("2"),
				SingleNoGpsCoordinate("3"),
				SingleItalyArezzo1("4"),
				SingleItalyArezzo2("5"),
				SingleItalyArezzo3("6"),
				SingleItalyArezzo4("7"),
				SingleItalyArezzo5("8"),
				SingleItalyArezzo6("9"),
				SingleItalyArezzo7("10"),
				SingleItalyArezzo8("11"),
				SingleItalyArezzo9("12"),
				SingleItalyArezzo9Duplicate("13"),
				SingleUnitedKingdom("14"),
				SingleSpain1("15"),
				SingleSpain2("16"),
				SingleNoGpsCoordinateAndNoPhotoTakenDate("17"),
				SingleNoPhotoTakenDate("18"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithNumericNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("01"),
				SingleItalyFlorence("02"),
				SingleNoGpsCoordinate("03"),
				SingleItalyArezzo1("04"),
				SingleItalyArezzo2("05"),
				SingleItalyArezzo3("06"),
				SingleItalyArezzo4("07"),
				SingleItalyArezzo5("08"),
				SingleItalyArezzo6("09"),
				SingleItalyArezzo7("10"),
				SingleItalyArezzo8("11"),
				SingleItalyArezzo9("12"),
				SingleItalyArezzo9Duplicate("13"),
				SingleUnitedKingdom("14"),
				SingleSpain1("15"),
				SingleSpain2("16"),
				SingleNoGpsCoordinateAndNoPhotoTakenDate("17"),
				SingleNoPhotoTakenDate("18"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithNumericNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("10"),
				SingleItalyFlorence("11"),
				SingleNoGpsCoordinate("12"),
				SingleItalyArezzo1("13"),
				SingleItalyArezzo2("14"),
				SingleItalyArezzo3("15"),
				SingleItalyArezzo4("16"),
				SingleItalyArezzo5("17"),
				SingleItalyArezzo6("18"),
				SingleItalyArezzo7("19"),
				SingleItalyArezzo8("20"),
				SingleItalyArezzo9("21"),
				SingleItalyArezzo9Duplicate("22"),
				SingleUnitedKingdom("23"),
				SingleSpain1("24"),
				SingleSpain2("25"),
				SingleNoGpsCoordinateAndNoPhotoTakenDate("26"),
				SingleNoPhotoTakenDate("27"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithDayNamingAndDuplicateNewNamesUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Day, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("2005.08.13"),
				SingleItalyFlorence("2005.12.14"),
				SingleNoGpsCoordinate("2008.07.16"),
				SingleItalyArezzo1("2008.10.22-1"),
				SingleItalyArezzo2("2008.10.22-2"),
				SingleItalyArezzo3("2008.10.22-3"),
				SingleItalyArezzo4("2008.10.22-4"),
				SingleItalyArezzo5("2008.10.22-5"),
				SingleItalyArezzo6("2008.10.22-6"),
				SingleItalyArezzo7("2008.10.22-7"),
				SingleItalyArezzo8("2008.10.22-8"),
				SingleItalyArezzo9("2008.10.22-9"),
				SingleItalyArezzo9Duplicate("2008.10.22-10"),
				SingleUnitedKingdom("2012.06.22"),
				SingleSpain1("2015.04.10-1"),
				SingleSpain2("2015.04.10-2"),
				SingleNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SingleNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithDayNamingAndDuplicateNewNamesUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Day, FolderProcessType.Single,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("2005.08.13"),
				SingleItalyFlorence("2005.12.14"),
				SingleNoGpsCoordinate("2008.07.16"),
				SingleItalyArezzo1("2008.10.22-01"),
				SingleItalyArezzo2("2008.10.22-02"),
				SingleItalyArezzo3("2008.10.22-03"),
				SingleItalyArezzo4("2008.10.22-04"),
				SingleItalyArezzo5("2008.10.22-05"),
				SingleItalyArezzo6("2008.10.22-06"),
				SingleItalyArezzo7("2008.10.22-07"),
				SingleItalyArezzo8("2008.10.22-08"),
				SingleItalyArezzo9("2008.10.22-09"),
				SingleItalyArezzo9Duplicate("2008.10.22-10"),
				SingleUnitedKingdom("2012.06.22"),
				SingleSpain1("2015.04.10-1"),
				SingleSpain2("2015.04.10-2"),
				SingleNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SingleNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithDayNamingAndDuplicateNewNamesUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Day, FolderProcessType.Single,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("2005.08.13"),
				SingleItalyFlorence("2005.12.14"),
				SingleNoGpsCoordinate("2008.07.16"),
				SingleItalyArezzo1("2008.10.22-10"),
				SingleItalyArezzo2("2008.10.22-11"),
				SingleItalyArezzo3("2008.10.22-12"),
				SingleItalyArezzo4("2008.10.22-13"),
				SingleItalyArezzo5("2008.10.22-14"),
				SingleItalyArezzo6("2008.10.22-15"),
				SingleItalyArezzo7("2008.10.22-16"),
				SingleItalyArezzo8("2008.10.22-17"),
				SingleItalyArezzo9("2008.10.22-18"),
				SingleItalyArezzo9Duplicate("2008.10.22-19"),
				SingleUnitedKingdom("2012.06.22"),
				SingleSpain1("2015.04.10-1"),
				SingleSpain2("2015.04.10-2"),
				SingleNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SingleNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithDateTimeWithMinutesNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.DateTimeWithMinutes, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("2005.08.13_09.47"),
				SingleItalyFlorence("2005.12.14_14.39"),
				SingleNoGpsCoordinate("2008.07.16_11.33"),
				SingleItalyArezzo1("2008.10.22_16.28"),
				SingleItalyArezzo2("2008.10.22_16.29"),
				SingleItalyArezzo3("2008.10.22_16.38"),
				SingleItalyArezzo4("2008.10.22_16.43"),
				SingleItalyArezzo5("2008.10.22_16.44"),
				SingleItalyArezzo6("2008.10.22_16.46"),
				SingleItalyArezzo7("2008.10.22_16.52"),
				SingleItalyArezzo8("2008.10.22_16.55"),
				SingleItalyArezzo9("2008.10.22_17.00-1"),
				SingleItalyArezzo9Duplicate("2008.10.22_17.00-2"),
				SingleUnitedKingdom("2012.06.22_19.52"),
				SingleSpain1("2015.04.10_20.12-1"),
				SingleSpain2("2015.04.10_20.12-2"),
				SingleNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SingleNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithDateTimeWithSecondsNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.DateTimeWithSeconds, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("2005.08.13_09.47.23"),
				SingleItalyFlorence("2005.12.14_14.39.47"),
				SingleNoGpsCoordinate("2008.07.16_11.33.20"),
				SingleItalyArezzo1("2008.10.22_16.28.39"),
				SingleItalyArezzo2("2008.10.22_16.29.49"),
				SingleItalyArezzo3("2008.10.22_16.38.20"),
				SingleItalyArezzo4("2008.10.22_16.43.21"),
				SingleItalyArezzo5("2008.10.22_16.44.01"),
				SingleItalyArezzo6("2008.10.22_16.46.53"),
				SingleItalyArezzo7("2008.10.22_16.52.15"),
				SingleItalyArezzo8("2008.10.22_16.55.37"),
				SingleItalyArezzo9("2008.10.22_17.00.07-1"),
				SingleItalyArezzo9Duplicate("2008.10.22_17.00.07-2"),
				SingleUnitedKingdom("2012.06.22_19.52.31"),
				SingleSpain1("2015.04.10_20.12.23-1"),
				SingleSpain2("2015.04.10_20.12.23-2"),
				SingleNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SingleNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	#endregion

	#region Only Reverse Geocode - Address

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithAddressNamingAndDuplicateNewNamesUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Address, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya("Nakuru", true, "Nakuru"),
				SingleItalyFlorence("Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SingleItalyArezzo1("Toscana-Province of Arezzo-1", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo2("Toscana-Province of Arezzo-2", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo3("Toscana-Province of Arezzo-3", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo4("Toscana-Province of Arezzo-4", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo5("Toscana-Province of Arezzo-5", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo6("Toscana-Province of Arezzo-6", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo7("Toscana-Province of Arezzo-7", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo8("Toscana-Province of Arezzo-8", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9("Toscana-Province of Arezzo-9", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9Duplicate("Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SingleUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SingleSpain1("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SingleSpain2("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithAddressNamingAndDuplicateNewNamesUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Address, FolderProcessType.Single,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya("Nakuru", true, "Nakuru"),
				SingleItalyFlorence("Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SingleItalyArezzo1("Toscana-Province of Arezzo-01", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo2("Toscana-Province of Arezzo-02", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo3("Toscana-Province of Arezzo-03", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo4("Toscana-Province of Arezzo-04", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo5("Toscana-Province of Arezzo-05", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo6("Toscana-Province of Arezzo-06", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo7("Toscana-Province of Arezzo-07", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo8("Toscana-Province of Arezzo-08", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9("Toscana-Province of Arezzo-09", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9Duplicate("Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SingleUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SingleSpain1("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SingleSpain2("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithAddressNamingAndDuplicateNewNamesUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Address, FolderProcessType.Single,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya("Nakuru", true, "Nakuru"),
				SingleItalyFlorence("Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SingleItalyArezzo1("Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo2("Toscana-Province of Arezzo-11", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo3("Toscana-Province of Arezzo-12", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo4("Toscana-Province of Arezzo-13", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo5("Toscana-Province of Arezzo-14", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo6("Toscana-Province of Arezzo-15", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo7("Toscana-Province of Arezzo-16", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo8("Toscana-Province of Arezzo-17", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9("Toscana-Province of Arezzo-18", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9Duplicate("Toscana-Province of Arezzo-19", true, "Toscana", "Province of Arezzo"),
				SingleUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SingleSpain1("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SingleSpain2("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	#endregion

	#region Combine Photo Taken Date & Reverse Geocode - Address

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SingleFolderWithDateTimeWithDayAddressNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.DayAddress, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya("2005.08.13-Nakuru", true, "Nakuru"),
				SingleItalyFlorence("2005.12.14-Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SingleItalyArezzo1("2008.10.22-Toscana-Province of Arezzo-1", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo2("2008.10.22-Toscana-Province of Arezzo-2", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo3("2008.10.22-Toscana-Province of Arezzo-3", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo4("2008.10.22-Toscana-Province of Arezzo-4", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo5("2008.10.22-Toscana-Province of Arezzo-5", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo6("2008.10.22-Toscana-Province of Arezzo-6", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo7("2008.10.22-Toscana-Province of Arezzo-7", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo8("2008.10.22-Toscana-Province of Arezzo-8", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9("2008.10.22-Toscana-Province of Arezzo-9", true, "Toscana", "Province of Arezzo"),
				SingleItalyArezzo9Duplicate("2008.10.22-Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SingleUnitedKingdom("2012.06.22-England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SingleSpain1("2015.04.10-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SingleSpain2("2015.04.10-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	#endregion

	#endregion

	#region Flatten All Subfolders

	#region Only Photo Taken Date

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithNumericNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("1"),
				SubFoldersItalyFlorence("2"),
				SubFoldersNoGpsCoordinate("3"),
				SubFoldersItalyArezzo1("4"),
				SubFoldersItalyArezzo2("5"),
				SubFoldersItalyArezzo3("6"),
				SubFoldersItalyArezzo4("7"),
				SubFoldersItalyArezzo5("8"),
				SubFoldersItalyArezzo6("9"),
				SubFoldersItalyArezzo7("10"),
				SubFoldersItalyArezzo8("11"),
				SubFoldersItalyArezzo9("12"),
				SubFoldersItalyArezzo9Duplicate("13"),
				SubFoldersUnitedKingdom("14"),
				SubFoldersSpain1("15"),
				SubFoldersSpain2("16"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("17"),
				SubFoldersNoPhotoTakenDate("18"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithNumericNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("01"),
				SubFoldersItalyFlorence("02"),
				SubFoldersNoGpsCoordinate("03"),
				SubFoldersItalyArezzo1("04"),
				SubFoldersItalyArezzo2("05"),
				SubFoldersItalyArezzo3("06"),
				SubFoldersItalyArezzo4("07"),
				SubFoldersItalyArezzo5("08"),
				SubFoldersItalyArezzo6("09"),
				SubFoldersItalyArezzo7("10"),
				SubFoldersItalyArezzo8("11"),
				SubFoldersItalyArezzo9("12"),
				SubFoldersItalyArezzo9Duplicate("13"),
				SubFoldersUnitedKingdom("14"),
				SubFoldersSpain1("15"),
				SubFoldersSpain2("16"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("17"),
				SubFoldersNoPhotoTakenDate("18"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithNumericNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("10"),
				SubFoldersItalyFlorence("11"),
				SubFoldersNoGpsCoordinate("12"),
				SubFoldersItalyArezzo1("13"),
				SubFoldersItalyArezzo2("14"),
				SubFoldersItalyArezzo3("15"),
				SubFoldersItalyArezzo4("16"),
				SubFoldersItalyArezzo5("17"),
				SubFoldersItalyArezzo6("18"),
				SubFoldersItalyArezzo7("19"),
				SubFoldersItalyArezzo8("20"),
				SubFoldersItalyArezzo9("21"),
				SubFoldersItalyArezzo9Duplicate("22"),
				SubFoldersUnitedKingdom("23"),
				SubFoldersSpain1("24"),
				SubFoldersSpain2("25"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("26"),
				SubFoldersNoPhotoTakenDate("27"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithDayNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13"),
				SubFoldersItalyFlorence("2005.12.14"),
				SubFoldersNoGpsCoordinate("2008.07.16"),
				SubFoldersItalyArezzo1("2008.10.22-1"),
				SubFoldersItalyArezzo2("2008.10.22-2"),
				SubFoldersItalyArezzo3("2008.10.22-3"),
				SubFoldersItalyArezzo4("2008.10.22-4"),
				SubFoldersItalyArezzo5("2008.10.22-5"),
				SubFoldersItalyArezzo6("2008.10.22-6"),
				SubFoldersItalyArezzo7("2008.10.22-7"),
				SubFoldersItalyArezzo8("2008.10.22-8"),
				SubFoldersItalyArezzo9("2008.10.22-9"),
				SubFoldersItalyArezzo9Duplicate("2008.10.22-10"),
				SubFoldersUnitedKingdom("2012.06.22"),
				SubFoldersSpain1("2015.04.10-1"),
				SubFoldersSpain2("2015.04.10-2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithDayNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13"),
				SubFoldersItalyFlorence("2005.12.14"),
				SubFoldersNoGpsCoordinate("2008.07.16"),
				SubFoldersItalyArezzo1("2008.10.22-01"),
				SubFoldersItalyArezzo2("2008.10.22-02"),
				SubFoldersItalyArezzo3("2008.10.22-03"),
				SubFoldersItalyArezzo4("2008.10.22-04"),
				SubFoldersItalyArezzo5("2008.10.22-05"),
				SubFoldersItalyArezzo6("2008.10.22-06"),
				SubFoldersItalyArezzo7("2008.10.22-07"),
				SubFoldersItalyArezzo8("2008.10.22-08"),
				SubFoldersItalyArezzo9("2008.10.22-09"),
				SubFoldersItalyArezzo9Duplicate("2008.10.22-10"),
				SubFoldersUnitedKingdom("2012.06.22"),
				SubFoldersSpain1("2015.04.10-1"),
				SubFoldersSpain2("2015.04.10-2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithDayNamingUsingPaddingZeroCharacterAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13"),
				SubFoldersItalyFlorence("2005.12.14"),
				SubFoldersNoGpsCoordinate("2008.07.16"),
				SubFoldersItalyArezzo1("2008.10.22-10"),
				SubFoldersItalyArezzo2("2008.10.22-11"),
				SubFoldersItalyArezzo3("2008.10.22-12"),
				SubFoldersItalyArezzo4("2008.10.22-13"),
				SubFoldersItalyArezzo5("2008.10.22-14"),
				SubFoldersItalyArezzo6("2008.10.22-15"),
				SubFoldersItalyArezzo7("2008.10.22-16"),
				SubFoldersItalyArezzo8("2008.10.22-17"),
				SubFoldersItalyArezzo9("2008.10.22-18"),
				SubFoldersItalyArezzo9Duplicate("2008.10.22-19"),
				SubFoldersUnitedKingdom("2012.06.22"),
				SubFoldersSpain1("2015.04.10-1"),
				SubFoldersSpain2("2015.04.10-2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithDateTimeWithMinutesNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.DateTimeWithMinutes, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13_09.47"),
				SubFoldersItalyFlorence("2005.12.14_14.39"),
				SubFoldersNoGpsCoordinate("2008.07.16_11.33"),
				SubFoldersItalyArezzo1("2008.10.22_16.28"),
				SubFoldersItalyArezzo2("2008.10.22_16.29"),
				SubFoldersItalyArezzo3("2008.10.22_16.38"),
				SubFoldersItalyArezzo4("2008.10.22_16.43"),
				SubFoldersItalyArezzo5("2008.10.22_16.44"),
				SubFoldersItalyArezzo6("2008.10.22_16.46"),
				SubFoldersItalyArezzo7("2008.10.22_16.52"),
				SubFoldersItalyArezzo8("2008.10.22_16.55"),
				SubFoldersItalyArezzo9("2008.10.22_17.00-1"),
				SubFoldersItalyArezzo9Duplicate("2008.10.22_17.00-2"),
				SubFoldersUnitedKingdom("2012.06.22_19.52"),
				SubFoldersSpain1("2015.04.10_20.12-1"),
				SubFoldersSpain2("2015.04.10_20.12-2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithDateTimeWithSecondsNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.DateTimeWithSeconds, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13_09.47.23"),
				SubFoldersItalyFlorence("2005.12.14_14.39.47"),
				SubFoldersNoGpsCoordinate("2008.07.16_11.33.20"),
				SubFoldersItalyArezzo1("2008.10.22_16.28.39"),
				SubFoldersItalyArezzo2("2008.10.22_16.29.49"),
				SubFoldersItalyArezzo3("2008.10.22_16.38.20"),
				SubFoldersItalyArezzo4("2008.10.22_16.43.21"),
				SubFoldersItalyArezzo5("2008.10.22_16.44.01"),
				SubFoldersItalyArezzo6("2008.10.22_16.46.53"),
				SubFoldersItalyArezzo7("2008.10.22_16.52.15"),
				SubFoldersItalyArezzo8("2008.10.22_16.55.37"),
				SubFoldersItalyArezzo9("2008.10.22_17.00.07-1"),
				SubFoldersItalyArezzo9Duplicate("2008.10.22_17.00.07-2"),
				SubFoldersUnitedKingdom("2012.06.22_19.52.31"),
				SubFoldersSpain1("2015.04.10_20.12.23-1"),
				SubFoldersSpain2("2015.04.10_20.12.23-2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoPhotoTakenDateWithDefaultName(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	#endregion

	#region Only Reverse Geocode - Address

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithAddressNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("Nakuru", true, "Nakuru"),
				SubFoldersItalyFlorence("Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("Toscana-Province of Arezzo-1", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("Toscana-Province of Arezzo-2", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("Toscana-Province of Arezzo-3", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("Toscana-Province of Arezzo-4", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("Toscana-Province of Arezzo-5", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("Toscana-Province of Arezzo-6", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("Toscana-Province of Arezzo-7", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("Toscana-Province of Arezzo-8", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("Toscana-Province of Arezzo-9", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersSpain1("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithAddressNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("Nakuru", true, "Nakuru"),
				SubFoldersItalyFlorence("Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("Toscana-Province of Arezzo-01", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("Toscana-Province of Arezzo-02", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("Toscana-Province of Arezzo-03", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("Toscana-Province of Arezzo-04", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("Toscana-Province of Arezzo-05", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("Toscana-Province of Arezzo-06", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("Toscana-Province of Arezzo-07", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("Toscana-Province of Arezzo-08", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("Toscana-Province of Arezzo-09", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersSpain1("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithAddressNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("Nakuru", true, "Nakuru"),
				SubFoldersItalyFlorence("Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("Toscana-Province of Arezzo-11", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("Toscana-Province of Arezzo-12", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("Toscana-Province of Arezzo-13", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("Toscana-Province of Arezzo-14", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("Toscana-Province of Arezzo-15", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("Toscana-Province of Arezzo-16", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("Toscana-Province of Arezzo-17", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("Toscana-Province of Arezzo-18", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("Toscana-Province of Arezzo-19", true, "Toscana", "Province of Arezzo"),
				SubFoldersUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersSpain1("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	#endregion

	#region Combine Photo Taken Date & Reverse Geocode - Address

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersWithDayAddressNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.DayAddress, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13-Nakuru", true, "Nakuru"),
				SubFoldersItalyFlorence("2005.12.14-Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("2008.10.22-Toscana-Province of Arezzo-1", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("2008.10.22-Toscana-Province of Arezzo-2", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("2008.10.22-Toscana-Province of Arezzo-3", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("2008.10.22-Toscana-Province of Arezzo-4", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("2008.10.22-Toscana-Province of Arezzo-5", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("2008.10.22-Toscana-Province of Arezzo-6", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("2008.10.22-Toscana-Province of Arezzo-7", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("2008.10.22-Toscana-Province of Arezzo-8", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("2008.10.22-Toscana-Province of Arezzo-9", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("2008.10.22-Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersUnitedKingdom("2012.06.22-England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersSpain1("2015.04.10-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("2015.04.10-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	#endregion

	#region Group By Folder

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersAndGroupByYearMonthDayWithNumericNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, false, GroupByFolderType.YearMonthDay),
			new List<PhotoCsv>
			{
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("1"),
				SubFoldersNoPhotoTakenDate("2"),
				SubFoldersUnitedKingdom("2012/06/22/1"),
				SubFoldersKenya("2005/08/13/1"),
				SubFoldersSpain1("2015/04/10/1"),
				SubFoldersSpain2("2015/04/10/2"),
				SubFoldersItalyFlorence("2005/12/14/1"),
				SubFoldersNoGpsCoordinate("2008/07/16/1"),
				SubFoldersItalyArezzo1("2008/10/22/1"),
				SubFoldersItalyArezzo2("2008/10/22/2"),
				SubFoldersItalyArezzo3("2008/10/22/3"),
				SubFoldersItalyArezzo4("2008/10/22/4"),
				SubFoldersItalyArezzo5("2008/10/22/5"),
				SubFoldersItalyArezzo6("2008/10/22/6"),
				SubFoldersItalyArezzo7("2008/10/22/7"),
				SubFoldersItalyArezzo8("2008/10/22/8"),
				SubFoldersItalyArezzo9("2008/10/22/9"),
				SubFoldersItalyArezzo9Duplicate("2008/10/22/10"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 6)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersAndGroupByYearMonthWithDateTimeWithMinutesNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.DateTimeWithMinutes, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, false, GroupByFolderType.YearMonth),
			new List<PhotoCsv>
			{
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoPhotoTakenDateWithDefaultName(),
				SubFoldersUnitedKingdom("2012/06/2012.06.22_19.52"),
				SubFoldersKenya("2005/08/2005.08.13_09.47"),
				SubFoldersSpain1("2015/04/2015.04.10_20.12-1"),
				SubFoldersSpain2("2015/04/2015.04.10_20.12-2"),
				SubFoldersItalyFlorence("2005/12/2005.12.14_14.39"),
				SubFoldersNoGpsCoordinate("2008/07/2008.07.16_11.33"),
				SubFoldersItalyArezzo1("2008/10/2008.10.22_16.28"),
				SubFoldersItalyArezzo2("2008/10/2008.10.22_16.29"),
				SubFoldersItalyArezzo3("2008/10/2008.10.22_16.38"),
				SubFoldersItalyArezzo4("2008/10/2008.10.22_16.43"),
				SubFoldersItalyArezzo5("2008/10/2008.10.22_16.44"),
				SubFoldersItalyArezzo6("2008/10/2008.10.22_16.46"),
				SubFoldersItalyArezzo7("2008/10/2008.10.22_16.52"),
				SubFoldersItalyArezzo8("2008/10/2008.10.22_16.55"),
				SubFoldersItalyArezzo9("2008/10/2008.10.22_17.00-1"),
				SubFoldersItalyArezzo9Duplicate("2008/10/2008.10.22_17.00-2"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 6)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersAndGroupByAddressFlatWithDayNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, false, GroupByFolderType.AddressFlat,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersUnitedKingdom("England-South East England-Windsor and Maidenhead/2012.06.22", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersKenya("Nakuru/2005.08.13", true, "Nakuru"),
				SubFoldersSpain1("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area/2015.04.10-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area/2015.04.10-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersItalyFlorence("Toscana-Province of Florence/2005.12.14", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("Toscana-Province of Arezzo/2008.10.22-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("Toscana-Province of Arezzo/2008.10.22-11", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("Toscana-Province of Arezzo/2008.10.22-12", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("Toscana-Province of Arezzo/2008.10.22-13", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("Toscana-Province of Arezzo/2008.10.22-14", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("Toscana-Province of Arezzo/2008.10.22-15", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("Toscana-Province of Arezzo/2008.10.22-16", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("Toscana-Province of Arezzo/2008.10.22-17", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("Toscana-Province of Arezzo/2008.10.22-18", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("Toscana-Province of Arezzo/2008.10.22-19", true, "Toscana", "Province of Arezzo"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoGpsCoordinate("2008.07.16"),
				SubFoldersNoPhotoTakenDate("NoPhotoTakenDate"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 5)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> FlattenAllSubFoldersAndGroupByAddressHierarchyWithDayNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, false, GroupByFolderType.AddressHierarchy,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersUnitedKingdom("England/South East England/Windsor and Maidenhead/2012.06.22", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersKenya("Nakuru/2005.08.13", true, "Nakuru"),
				SubFoldersSpain1("Community of Madrid/Comunidad de Madrid/Madrid Metropolitan Area/2015.04.10-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("Community of Madrid/Comunidad de Madrid/Madrid Metropolitan Area/2015.04.10-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersItalyFlorence("Toscana/Province of Florence/2005.12.14", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("Toscana/Province of Arezzo/2008.10.22-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("Toscana/Province of Arezzo/2008.10.22-11", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("Toscana/Province of Arezzo/2008.10.22-12", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("Toscana/Province of Arezzo/2008.10.22-13", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("Toscana/Province of Arezzo/2008.10.22-14", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("Toscana/Province of Arezzo/2008.10.22-15", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("Toscana/Province of Arezzo/2008.10.22-16", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("Toscana/Province of Arezzo/2008.10.22-17", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("Toscana/Province of Arezzo/2008.10.22-18", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("Toscana/Province of Arezzo/2008.10.22-19", true, "Toscana", "Province of Arezzo"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoGpsCoordinate("2008.07.16"),
				SubFoldersNoPhotoTakenDate("NoPhotoTakenDate"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 5)
		}
	};

	#endregion

	#endregion

	#region Subfolders Preserve Folder Hierarchy

	#region Only Photo Taken Date

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithNumericNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("1"),
				SubFoldersUnitedKingdom("2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("3"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/1"),
				SubFoldersNoGpsCoordinate("ItalyFolder/Florence/2"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/1"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/2"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/3"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/4"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/5"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/6"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/7"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/8"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/9"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/10"),
				SubFoldersSpain1("SpainFolder/1"),
				SubFoldersSpain2("SpainFolder/2"),
				SubFoldersNoPhotoTakenDate("SpainFolder/3"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithNumericNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("1"),
				SubFoldersUnitedKingdom("2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("3"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/1"),
				SubFoldersNoGpsCoordinate("ItalyFolder/Florence/2"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/01"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/02"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/03"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/04"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/05"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/06"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/07"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/08"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/09"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/10"),
				SubFoldersSpain1("SpainFolder/1"),
				SubFoldersSpain2("SpainFolder/2"),
				SubFoldersNoPhotoTakenDate("SpainFolder/3"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithNumericNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("1"),
				SubFoldersUnitedKingdom("2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("3"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/1"),
				SubFoldersNoGpsCoordinate("ItalyFolder/Florence/2"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/10"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/11"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/12"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/13"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/14"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/15"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/16"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/17"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/18"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/19"),
				SubFoldersSpain1("SpainFolder/1"),
				SubFoldersSpain2("SpainFolder/2"),
				SubFoldersNoPhotoTakenDate("SpainFolder/3"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithDayNamingAndDuplicateNewNamesUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13"),
				SubFoldersUnitedKingdom("2012.06.22"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersItalyFlorence("ItalyFolder/Florence/2005.12.14"),
				SubFoldersNoGpsCoordinate("ItalyFolder/Florence/2008.07.16"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/2008.10.22-1"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/2008.10.22-2"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/2008.10.22-3"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/2008.10.22-4"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/2008.10.22-5"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/2008.10.22-6"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/2008.10.22-7"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/2008.10.22-8"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/2008.10.22-9"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/2008.10.22-10"),
				SubFoldersSpain1("SpainFolder/2015.04.10-1"),
				SubFoldersSpain2("SpainFolder/2015.04.10-2"),
				SubFoldersNoPhotoTakenDate("SpainFolder/NoPhotoTakenDate"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithDayNamingAndDuplicateNewNamesUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13"),
				SubFoldersUnitedKingdom("2012.06.22"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersItalyFlorence("ItalyFolder/Florence/2005.12.14"),
				SubFoldersNoGpsCoordinate("ItalyFolder/Florence/2008.07.16"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/2008.10.22-01"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/2008.10.22-02"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/2008.10.22-03"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/2008.10.22-04"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/2008.10.22-05"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/2008.10.22-06"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/2008.10.22-07"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/2008.10.22-08"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/2008.10.22-09"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/2008.10.22-10"),
				SubFoldersSpain1("SpainFolder/2015.04.10-1"),
				SubFoldersSpain2("SpainFolder/2015.04.10-2"),
				SubFoldersNoPhotoTakenDate("SpainFolder/NoPhotoTakenDate"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithDayNamingAndDuplicateNewNamesUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13"),
				SubFoldersUnitedKingdom("2012.06.22"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersItalyFlorence("ItalyFolder/Florence/2005.12.14"),
				SubFoldersNoGpsCoordinate("ItalyFolder/Florence/2008.07.16"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/2008.10.22-10"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/2008.10.22-11"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/2008.10.22-12"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/2008.10.22-13"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/2008.10.22-14"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/2008.10.22-15"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/2008.10.22-16"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/2008.10.22-17"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/2008.10.22-18"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/2008.10.22-19"),
				SubFoldersSpain1("SpainFolder/2015.04.10-1"),
				SubFoldersSpain2("SpainFolder/2015.04.10-2"),
				SubFoldersNoPhotoTakenDate("SpainFolder/NoPhotoTakenDate"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
		}
	};

	#endregion

	#region Only Reverse Geocode - Address

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("Nakuru", true, "Nakuru"),
				SubFoldersUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/Toscana-Province of Arezzo-1", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/Toscana-Province of Arezzo-2", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/Toscana-Province of Arezzo-3", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/Toscana-Province of Arezzo-4", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/Toscana-Province of Arezzo-5", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/Toscana-Province of Arezzo-6", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/Toscana-Province of Arezzo-7", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/Toscana-Province of Arezzo-8", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/Toscana-Province of Arezzo-9", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersSpain1("SpainFolder/Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("SpainFolder/Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("Nakuru", true, "Nakuru"),
				SubFoldersUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/Toscana-Province of Arezzo-01", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/Toscana-Province of Arezzo-02", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/Toscana-Province of Arezzo-03", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/Toscana-Province of Arezzo-04", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/Toscana-Province of Arezzo-05", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/Toscana-Province of Arezzo-06", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/Toscana-Province of Arezzo-07", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/Toscana-Province of Arezzo-08", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/Toscana-Province of Arezzo-09", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersSpain1("SpainFolder/Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("SpainFolder/Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("Nakuru", true, "Nakuru"),
				SubFoldersUnitedKingdom("England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/Toscana-Province of Arezzo-11", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/Toscana-Province of Arezzo-12", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/Toscana-Province of Arezzo-13", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/Toscana-Province of Arezzo-14", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/Toscana-Province of Arezzo-15", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/Toscana-Province of Arezzo-16", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/Toscana-Province of Arezzo-17", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/Toscana-Province of Arezzo-18", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/Toscana-Province of Arezzo-19", true, "Toscana", "Province of Arezzo"),
				SubFoldersSpain1("SpainFolder/Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
				SubFoldersSpain2("SpainFolder/Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid", "Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3)
		}
	};

	#endregion

	#region Combine Photo Taken Date & Reverse Geocode - Address

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyWithDayAddressNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.DayAddress, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput, false,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya("2005.08.13-Nakuru", true, "Nakuru"),
				SubFoldersUnitedKingdom("2012.06.22-England-South East England-Windsor and Maidenhead", true, "England", "South East England", "Windsor and Maidenhead"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/2005.12.14-Toscana-Province of Florence", true, "Toscana", "Province of Florence"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-1", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-2", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-3", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-4", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-5", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-6", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-7", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-8", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-9", true, "Toscana", "Province of Arezzo"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/2008.10.22-Toscana-Province of Arezzo-10", true, "Toscana", "Province of Arezzo"),
				SubFoldersSpain1("SpainFolder/2015.04.10-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-1", true, "Community of Madrid", "Comunidad de Madrid",
					"Madrid Metropolitan Area"),
				SubFoldersSpain2("SpainFolder/2015.04.10-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area-2", true, "Community of Madrid", "Comunidad de Madrid",
					"Madrid Metropolitan Area"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3)
		}
	};

	#endregion

	#region Append Folder Name

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues>
		SubFoldersPreserveFolderHierarchyByAppendingFirstYearMonthAsPrefixToFolderNameWithNumericNamingUsingOnlySequentialNumbers = new()
		{
			{
				CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy,
					NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, false, folderAppendType: FolderAppendType.FirstYearMonth,
					folderAppendLocationType: FolderAppendLocationType.Prefix),
				new List<PhotoCsv>
				{
					SubFoldersKenya("1"),
					SubFoldersUnitedKingdom("2"),
					SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("3"),
					SubFoldersSpain1("2015.04-SpainFolder/1"),
					SubFoldersSpain2("2015.04-SpainFolder/2"),
					SubFoldersNoPhotoTakenDate("2015.04-SpainFolder/3"),
					SubFoldersItalyFlorence("ItalyFolder/2005.12-Florence/1"),
					SubFoldersNoGpsCoordinate("ItalyFolder/2005.12-Florence/2"),
					SubFoldersItalyArezzo1("ItalyFolder/2008.10-Arezzo/1"),
					SubFoldersItalyArezzo2("ItalyFolder/2008.10-Arezzo/2"),
					SubFoldersItalyArezzo3("ItalyFolder/2008.10-Arezzo/3"),
					SubFoldersItalyArezzo4("ItalyFolder/2008.10-Arezzo/4"),
					SubFoldersItalyArezzo5("ItalyFolder/2008.10-Arezzo/5"),
					SubFoldersItalyArezzo6("ItalyFolder/2008.10-Arezzo/6"),
					SubFoldersItalyArezzo7("ItalyFolder/2008.10-Arezzo/7"),
					SubFoldersItalyArezzo8("ItalyFolder/2008.10-Arezzo/8"),
					SubFoldersItalyArezzo9("ItalyFolder/2008.10-Arezzo/9"),
					SubFoldersItalyArezzo9Duplicate("ItalyFolder/2008.10-Arezzo/10"),
				},
				new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
			}
		};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> SubFoldersPreserveFolderHierarchyByAppendingDayRangeAsPrefixToFolderNameWithNumericNamingUsingOnlySequentialNumbers =
		new()
		{
			{
				CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.SubFoldersPreserveFolderHierarchy,
					NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, false, folderAppendType: FolderAppendType.DayRange,
					folderAppendLocationType: FolderAppendLocationType.Prefix),
				new List<PhotoCsv>
				{
					SubFoldersKenya("2005.08.13"),
					SubFoldersUnitedKingdom("2012.06.22"),
					SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
					SubFoldersSpain1("2015.04.10-2015.04.10-SpainFolder/2015.04.10-1"),
					SubFoldersSpain2("2015.04.10-2015.04.10-SpainFolder/2015.04.10-2"),
					SubFoldersNoPhotoTakenDate("2015.04.10-2015.04.10-SpainFolder/NoPhotoTakenDate"),
					SubFoldersItalyFlorence("ItalyFolder/2005.12.14-2008.07.16-Florence/2005.12.14"),
					SubFoldersNoGpsCoordinate("ItalyFolder/2005.12.14-2008.07.16-Florence/2008.07.16"),
					SubFoldersItalyArezzo1("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-1"),
					SubFoldersItalyArezzo2("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-2"),
					SubFoldersItalyArezzo3("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-3"),
					SubFoldersItalyArezzo4("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-4"),
					SubFoldersItalyArezzo5("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-5"),
					SubFoldersItalyArezzo6("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-6"),
					SubFoldersItalyArezzo7("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-7"),
					SubFoldersItalyArezzo8("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-8"),
					SubFoldersItalyArezzo9("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-9"),
					SubFoldersItalyArezzo9Duplicate("ItalyFolder/2008.10.22-2008.10.22-Arezzo/2008.10.22-10"),
				},
				new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
			}
		};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues>
		SubFoldersPreserveFolderHierarchyByAppendingMatchingMinimumAddressAsSuffixToFolderNameWithNumericNamingUsingOnlySequentialNumbers = new()
		{
			{
				CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.SubFoldersPreserveFolderHierarchy,
					NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, false,
					folderAppendType: FolderAppendType.MatchingMinimumAddress, folderAppendLocationType: FolderAppendLocationType.Suffix,
					reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
				new List<PhotoCsv>
				{
					SubFoldersKenya("2005.08.13", true, "Nakuru"),
					SubFoldersUnitedKingdom("2012.06.22", true, "England", "South East England", "Windsor and Maidenhead"),
					SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
					SubFoldersSpain1("SpainFolder-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area/2015.04.10-1", true, "Community of Madrid", "Comunidad de Madrid",
						"Madrid Metropolitan Area"),
					SubFoldersSpain2("SpainFolder-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area/2015.04.10-2", true, "Community of Madrid", "Comunidad de Madrid",
						"Madrid Metropolitan Area"),
					SubFoldersNoPhotoTakenDate("SpainFolder-Community of Madrid-Comunidad de Madrid-Madrid Metropolitan Area/NoPhotoTakenDate"),
					SubFoldersItalyFlorence("ItalyFolder/Florence-Toscana-Province of Florence/2005.12.14", true, "Toscana", "Province of Florence"),
					SubFoldersNoGpsCoordinate("ItalyFolder/Florence-Toscana-Province of Florence/2008.07.16"),
					SubFoldersItalyArezzo1("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-1", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo2("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-2", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo3("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-3", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo4("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-4", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo5("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-5", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo6("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-6", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo7("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-7", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo8("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-8", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo9("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-9", true, "Toscana", "Province of Arezzo"),
					SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo-Toscana-Province of Arezzo/2008.10.22-10", true, "Toscana", "Province of Arezzo"),
				},
				new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
			}
		};

	#endregion

	#endregion

	#region No Photo Taken Date Actions & No Coordinate Action

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> NoPhotoTakenDateActionDontCopyToOutputAndNoCoordinateActionContinue = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.Continue),
			new List<PhotoCsv>
			{
				SingleKenya("1"),
				SingleItalyFlorence("2"),
				SingleNoGpsCoordinate("3"),
				SingleItalyArezzo1("4"),
				SingleItalyArezzo2("5"),
				SingleItalyArezzo3("6"),
				SingleItalyArezzo4("7"),
				SingleItalyArezzo5("8"),
				SingleItalyArezzo6("9"),
				SingleItalyArezzo7("10"),
				SingleItalyArezzo8("11"),
				SingleItalyArezzo9("12"),
				SingleItalyArezzo9Duplicate("13"),
				SingleUnitedKingdom("14"),
				SingleSpain1("15"),
				SingleSpain2("16"),
			},
			new ConsoleOutputValues(18, 16, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> NoPhotoTakenDateActionDontCopyToOutputAndNoCoordinateActionNotCopyingToOutput = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput),
			new List<PhotoCsv>
			{
				SingleKenya("1"),
				SingleItalyFlorence("2"),
				SingleItalyArezzo1("3"),
				SingleItalyArezzo2("4"),
				SingleItalyArezzo3("5"),
				SingleItalyArezzo4("6"),
				SingleItalyArezzo5("7"),
				SingleItalyArezzo6("8"),
				SingleItalyArezzo7("9"),
				SingleItalyArezzo8("10"),
				SingleItalyArezzo9("11"),
				SingleItalyArezzo9Duplicate("12"),
				SingleUnitedKingdom("13"),
				SingleSpain1("14"),
				SingleSpain2("15"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> NoPhotoTakenDateActionContinueAndNoCoordinateActionDontCopyToOutput = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.DontCopyToOutput),
			new List<PhotoCsv>
			{
				SingleKenya("1"),
				SingleItalyFlorence("2"),
				SingleItalyArezzo1("3"),
				SingleItalyArezzo2("4"),
				SingleItalyArezzo3("5"),
				SingleItalyArezzo4("6"),
				SingleItalyArezzo5("7"),
				SingleItalyArezzo6("8"),
				SingleItalyArezzo7("9"),
				SingleItalyArezzo8("10"),
				SingleItalyArezzo9("11"),
				SingleItalyArezzo9Duplicate("12"),
				SingleUnitedKingdom("13"),
				SingleSpain1("14"),
				SingleSpain2("15"),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2)
		}
	};

	#endregion

	[Theory]
	[MemberData(nameof(SingleFolderWithNumericNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(SingleFolderWithNumericNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(SingleFolderWithNumericNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(SingleFolderWithDateTimeWithMinutesNaming))]
	[MemberData(nameof(SingleFolderWithDateTimeWithSecondsNaming))]
	[MemberData(nameof(SingleFolderWithDayNamingAndDuplicateNewNamesUsingOnlySequentialNumbers))]
	[MemberData(nameof(SingleFolderWithDayNamingAndDuplicateNewNamesUsingPaddingZeroCharacter))]
	[MemberData(nameof(SingleFolderWithDayNamingAndDuplicateNewNamesUsingAllNamesAreSameLength))]
	[MemberData(nameof(SingleFolderWithAddressNamingAndDuplicateNewNamesUsingOnlySequentialNumbers))]
	[MemberData(nameof(SingleFolderWithAddressNamingAndDuplicateNewNamesUsingPaddingZeroCharacter))]
	[MemberData(nameof(SingleFolderWithAddressNamingAndDuplicateNewNamesUsingAllNamesAreSameLength))]
	[MemberData(nameof(SingleFolderWithDateTimeWithDayAddressNaming))]
	[MemberData(nameof(FlattenAllSubFoldersWithNumericNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(FlattenAllSubFoldersWithNumericNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(FlattenAllSubFoldersWithNumericNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(FlattenAllSubFoldersWithAddressNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(FlattenAllSubFoldersWithAddressNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(FlattenAllSubFoldersWithAddressNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(FlattenAllSubFoldersWithDayNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(FlattenAllSubFoldersWithDayNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(FlattenAllSubFoldersWithDayNamingUsingPaddingZeroCharacterAllNamesAreSameLength))]
	[MemberData(nameof(FlattenAllSubFoldersWithDateTimeWithMinutesNaming))]
	[MemberData(nameof(FlattenAllSubFoldersWithDateTimeWithSecondsNaming))]
	[MemberData(nameof(FlattenAllSubFoldersWithDayAddressNaming))]
	[MemberData(nameof(FlattenAllSubFoldersAndGroupByYearMonthDayWithNumericNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(FlattenAllSubFoldersAndGroupByYearMonthWithDateTimeWithMinutesNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(FlattenAllSubFoldersAndGroupByAddressFlatWithDayNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(FlattenAllSubFoldersAndGroupByAddressHierarchyWithDayNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithNumericNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithNumericNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithNumericNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithDayNamingAndDuplicateNewNamesUsingOnlySequentialNumbers))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithDayNamingAndDuplicateNewNamesUsingPaddingZeroCharacter))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithDayNamingAndDuplicateNewNamesUsingAllNamesAreSameLength))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithDayAddressNaming))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyByAppendingFirstYearMonthAsPrefixToFolderNameWithNumericNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyByAppendingDayRangeAsPrefixToFolderNameWithNumericNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyByAppendingMatchingMinimumAddressAsSuffixToFolderNameWithNumericNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(NoPhotoTakenDateActionDontCopyToOutputAndNoCoordinateActionContinue))]
	[MemberData(nameof(NoPhotoTakenDateActionDontCopyToOutputAndNoCoordinateActionNotCopyingToOutput))]
	[MemberData(nameof(NoPhotoTakenDateActionContinueAndNoCoordinateActionDontCopyToOutput))]
	public async Task Running_With_Copy_Verb_Arguments_Should_Create_And_Verify_Photos_And_Report_Csv_On_File_System(string[] args, List<PhotoCsv> expectedPhotoCsvModels,
		ConsoleOutputValues expectedConsoleOutput)
	{
		DeleteOutputFolderIfExists();
		await ExecuteCopyShouldCreateAndVerifyResults(args, expectedPhotoCsvModels, expectedConsoleOutput, _csvReportFile, true);
		DeleteOutputFolderIfExists();
	}

	#region Dry Run

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> DryRunSingleFolder = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SingleFolder(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, true),
			new List<PhotoCsv>
			{
				SingleKenya("1"),
				SingleItalyFlorence("2"),
				SingleNoGpsCoordinate("3"),
				SingleItalyArezzo1("4"),
				SingleItalyArezzo2("5"),
				SingleItalyArezzo3("6"),
				SingleItalyArezzo4("7"),
				SingleItalyArezzo5("8"),
				SingleItalyArezzo6("9"),
				SingleItalyArezzo7("10"),
				SingleItalyArezzo8("11"),
				SingleItalyArezzo9("12"),
				SingleItalyArezzo9Duplicate("13"),
				SingleUnitedKingdom("14"),
				SingleSpain1("15"),
				SingleSpain2("16"),
				SingleNoGpsCoordinateAndNoPhotoTakenDate("17"),
				SingleNoPhotoTakenDate("18"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> DryRunFlattenAllSubFolders = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, true),
			new List<PhotoCsv>
			{
				SubFoldersKenya("1"),
				SubFoldersItalyFlorence("2"),
				SubFoldersNoGpsCoordinate("3"),
				SubFoldersItalyArezzo1("4"),
				SubFoldersItalyArezzo2("5"),
				SubFoldersItalyArezzo3("6"),
				SubFoldersItalyArezzo4("7"),
				SubFoldersItalyArezzo5("8"),
				SubFoldersItalyArezzo6("9"),
				SubFoldersItalyArezzo7("10"),
				SubFoldersItalyArezzo8("11"),
				SubFoldersItalyArezzo9("12"),
				SubFoldersItalyArezzo9Duplicate("13"),
				SubFoldersUnitedKingdom("14"),
				SubFoldersSpain1("15"),
				SubFoldersSpain2("16"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("17"),
				SubFoldersNoPhotoTakenDate("18"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2)
		}
	};

	public static TheoryData<string[], List<PhotoCsv>, ConsoleOutputValues> DryRunSubFoldersPreserveFolderHierarchy = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(OutputPath, TestImagesPathHelper.SubFolders(), NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, true),
			new List<PhotoCsv>
			{
				SubFoldersKenya("1"),
				SubFoldersUnitedKingdom("2"),
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("3"),
				SubFoldersItalyFlorence("ItalyFolder/Florence/1"),
				SubFoldersNoGpsCoordinate("ItalyFolder/Florence/2"),
				SubFoldersItalyArezzo1("ItalyFolder/Arezzo/1"),
				SubFoldersItalyArezzo2("ItalyFolder/Arezzo/2"),
				SubFoldersItalyArezzo3("ItalyFolder/Arezzo/3"),
				SubFoldersItalyArezzo4("ItalyFolder/Arezzo/4"),
				SubFoldersItalyArezzo5("ItalyFolder/Arezzo/5"),
				SubFoldersItalyArezzo6("ItalyFolder/Arezzo/6"),
				SubFoldersItalyArezzo7("ItalyFolder/Arezzo/7"),
				SubFoldersItalyArezzo8("ItalyFolder/Arezzo/8"),
				SubFoldersItalyArezzo9("ItalyFolder/Arezzo/9"),
				SubFoldersItalyArezzo9Duplicate("ItalyFolder/Arezzo/10"),
				SubFoldersSpain1("SpainFolder/1"),
				SubFoldersSpain2("SpainFolder/2"),
				SubFoldersNoPhotoTakenDate("SpainFolder/3"),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 3)
		}
	};

	#endregion

	[Theory]
	[MemberData(nameof(DryRunSingleFolder))]
	[MemberData(nameof(DryRunFlattenAllSubFolders))]
	[MemberData(nameof(DryRunSubFoldersPreserveFolderHierarchy))]
	public async Task Running_With_Dry_Run_Should_Report_Csv_On_File_System(string[] args, List<PhotoCsv> expectedPhotoCsvModels, ConsoleOutputValues expectedConsoleOutput)
	{
		DeleteDryRunFileIfExists();
		await ExecuteCopyShouldCreateAndVerifyResults(args, expectedPhotoCsvModels, expectedConsoleOutput, _dryRunCsvReportFile, false);
		DeleteDryRunFileIfExists();
	}

	private async Task ExecuteCopyShouldCreateAndVerifyResults(string[] args, List<PhotoCsv> expectedPhotoCsvModels, ConsoleOutputValues expectedConsoleOutput, FileInfo fileInfo, bool verifyFileExists)
	{
		var actualOutput = await RunMain(args);
		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(fileInfo);
		using (new AssertionScope())
		{
			actualConsoleOutput.Should().Be(expectedConsoleOutput);
			actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
			if (verifyFileExists)
			{
				foreach (var newPhoto in actualPhotoCsvModels.Select(actualPhotoCsvModel => new FileInfo(actualPhotoCsvModel.PhotoNewPath!)))
					newPhoto.Exists.Should().Be(true);
			}
		}
	}

	#region Utils

	private void DeleteOutputFolderIfExists()
	{
		if (_outputDirectory.Exists)
			_outputDirectory.Delete(true);
	}

	private void DeleteDryRunFileIfExists()
	{
		if (_dryRunCsvReportFile.Exists)
			_dryRunCsvReportFile.Delete();
	}

	#endregion
}
