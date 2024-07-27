namespace PhotoCli.Tests.EndToEndTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.EndToEndTests)]
public class CopyVerbCompanionsEndToEndTests : BaseCopyVerbEndToEndTests
{
	public CopyVerbCompanionsEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, List<string>> SingleFolderWithNumericNamingUsingOnlySequentialNumbersCompanions = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SingleFolderCompanions(), NamingStyle.Numeric, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.Continue),
			[
				SingleCompanionsHallstatt("1"),
				SingleCompanionsAmsterdam("2"),
				SingleCompanionsCopenhagen("3"),
				SingleCompanionsLeiden("4"),
				SingleCompanionsChios("5"),
			],
			new ConsoleOutputValues(5, 5, 5, CompanionsFound: 15, CompanionsCopied: 15),
			[
				"1.aae", "1.mov", "1.xmp",
				"2.aae", "2.mov", "2.xmp",
				"3.aae", "3.mov", "3.xmp",
				"4.aae", "4.mov", "4.xmp",
				"5.aae", "5.mov", "5.xmp",
			]
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, List<string>> FlattenAllSubFoldersWithNumericNamingUsingOnlySequentialNumbersCompanions = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFoldersCompanions(), NamingStyle.Numeric, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			[
				SubFoldersCompanionsHallstatt("1"),
				SubFoldersCompanionsAmsterdam("2"),
				SubFoldersCompanionsCopenhagen("3"),
				SubFoldersCompanionsLeiden("4"),
				SubFoldersCompanionsChios("5"),
			],
			new ConsoleOutputValues(5, 5, 5, CompanionsFound: 15, CompanionsCopied: 15),
			[
				"1.aae", "1.mov", "1.xmp",
				"2.aae", "2.mov", "2.xmp",
				"3.aae", "3.mov", "3.xmp",
				"4.aae", "4.mov", "4.xmp",
				"5.aae", "5.mov", "5.xmp",
			]
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, List<string>> FlattenAllSubFoldersAndGroupByYearMonthDayWithNumericNamingUsingOnlySequentialNumbersCompanions = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFoldersCompanions(), NamingStyle.Numeric, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, isDryRun: false, groupByFolderType: GroupByFolderType.YearMonthDay),
			[
				SubFoldersCompanionsHallstatt("2023/10/01/1"),
				SubFoldersCompanionsAmsterdam("2023/10/28/1"),
				SubFoldersCompanionsCopenhagen("2023/11/11/1"),
				SubFoldersCompanionsLeiden("2024/03/02/1"),
				SubFoldersCompanionsChios("2024/05/29/1"),
			],
			new ConsoleOutputValues(5, 5, 5, DirectoriesCreated: 5, CompanionsFound: 15, CompanionsCopied: 15),
			[
				"2023/10/01/1.aae", "2023/10/01/1.mov", "2023/10/01/1.xmp",
				"2023/10/28/1.aae", "2023/10/28/1.mov", "2023/10/28/1.xmp",
				"2023/11/11/1.aae", "2023/11/11/1.mov", "2023/11/11/1.xmp",
				"2024/03/02/1.aae", "2024/03/02/1.mov", "2024/03/02/1.xmp",
				"2024/05/29/1.aae", "2024/05/29/1.mov", "2024/05/29/1.xmp",
			]
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, List<string>> SubFoldersPreserveFolderHierarchyWithNumericNamingUsingOnlySequentialNumbersCompanions = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFoldersCompanions(), NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue),
			[
				SubFoldersCompanionsAmsterdam("Netherlands/1"),
				SubFoldersCompanionsLeiden("Netherlands/2"),
				SubFoldersCompanionsChios("Summer/1"),
				SubFoldersCompanionsHallstatt("Winter/1"),
				SubFoldersCompanionsCopenhagen("Winter/2"),

			],
			new ConsoleOutputValues(5, 5, 5, DirectoriesCreated: 3, CompanionsFound: 15, CompanionsCopied: 15),
			[
				"Netherlands/1.aae", "Netherlands/1.mov", "Netherlands/1.xmp",
				"Netherlands/2.aae", "Netherlands/2.mov", "Netherlands/2.xmp",
				"Summer/1.aae", "Summer/1.mov", "Summer/1.xmp",
				"Winter/1.aae", "Winter/1.mov", "Winter/1.xmp",
				"Winter/2.aae", "Winter/2.mov", "Winter/2.xmp",
			]
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, List<string>> SubFoldersPreserveFolderHierarchyByAppendingFirstYearMonthAsPrefixToFolderNameWithNumericNamingUsingOnlySequentialNumbersCompanions = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFoldersCompanions(), NamingStyle.Numeric, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, folderAppendType: FolderAppendType.FirstYearMonth,
				folderAppendLocationType: FolderAppendLocationType.Prefix),
			[
				SubFoldersCompanionsAmsterdam("2023.10-Netherlands/1"),
				SubFoldersCompanionsLeiden("2023.10-Netherlands/2"),
				SubFoldersCompanionsHallstatt("2023.10-Winter/1"),
				SubFoldersCompanionsCopenhagen("2023.10-Winter/2"),
				SubFoldersCompanionsChios("2024.05-Summer/1"),
			],
			new ConsoleOutputValues(5, 5, 5, DirectoriesCreated: 3, CompanionsFound: 15, CompanionsCopied: 15),
			[
				"2023.10-Netherlands/1.aae", "2023.10-Netherlands/1.mov", "2023.10-Netherlands/1.xmp",
				"2023.10-Netherlands/2.aae", "2023.10-Netherlands/2.mov", "2023.10-Netherlands/2.xmp",
				"2023.10-Winter/1.aae", "2023.10-Winter/1.mov", "2023.10-Winter/1.xmp",
				"2023.10-Winter/2.aae", "2023.10-Winter/2.mov", "2023.10-Winter/2.xmp",
				"2024.05-Summer/1.aae", "2024.05-Summer/1.mov", "2024.05-Summer/1.xmp",
			]
		}
	};

	[Theory]
	[MemberData(nameof(SingleFolderWithNumericNamingUsingOnlySequentialNumbersCompanions))]
	[MemberData(nameof(FlattenAllSubFoldersWithNumericNamingUsingOnlySequentialNumbersCompanions))]
	[MemberData(nameof(FlattenAllSubFoldersAndGroupByYearMonthDayWithNumericNamingUsingOnlySequentialNumbersCompanions))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithNumericNamingUsingOnlySequentialNumbersCompanions))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyByAppendingFirstYearMonthAsPrefixToFolderNameWithNumericNamingUsingOnlySequentialNumbersCompanions))]
	public async Task Companions_Running_With_Copy_Verb_Arguments_Should_Create_And_Verify_Photos_And_Report_Csv_On_File_System(ICollection<string> args, List<PhotoCsv> expectedPhotoCsvModels,
		ConsoleOutputValues expectedConsoleOutput, List<string> expectedCompanionFiles)
	{
		var outputFolder = OutputFolderForE2ETestPrivateToEachTest();
		CommandLineArgumentsFakes.AddOutputPathOptions(outputFolder, args);
		var csvReportFile = new FileInfo(Path.Combine(outputFolder, ToolOptionFakes.CsvReportFileName));
		var (actualConsoleOutput, actualPhotoCsvModels) = await ExecuteCopy(args.ToArray(), csvReportFile);
		using (new AssertionScope())
		{
			actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
			actualConsoleOutput.Should().Be(expectedConsoleOutput);
			VerifyCsvModelsNewPathExists(actualPhotoCsvModels, outputFolder);
			VerifyExpectedFilesOnOutput(expectedCompanionFiles, outputFolder);
		}

		DeleteOutput(outputFolder);
	}
}
