namespace PhotoCli.Tests.EndToEndTests;

public class CopyVerbAddressEndToEndTests : BaseCopyVerbEndToEndTests
{
	public CopyVerbAddressEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	#region Single FolderFlattenAllSubFoldersWithDayNamingUsingPaddingZeroCharacterAllNamesAreSameLength

	#region Only Reverse Geocode - Address

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SingleFolderWithAddressNamingAndDuplicateNewNamesUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SingleFolder(), NamingStyle.Address, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya(),
				SingleItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.Address)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SingleFolderWithAddressNamingAndDuplicateNewNamesUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SingleFolder(), NamingStyle.Address, FolderProcessType.Single,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya(),
				SingleItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.Address)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SingleFolderWithAddressNamingAndDuplicateNewNamesUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SingleFolder(), NamingStyle.Address, FolderProcessType.Single,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya(),
				SingleItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.Address)
		}
	};

	#endregion

	#region Combine Photo Taken Date & Reverse Geocode - Address

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SingleFolderWithDateTimeWithDayAddressNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SingleFolder(), NamingStyle.DayAddress, FolderProcessType.Single,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SingleKenya(),
				SingleItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.DayAddress)
		}
	};

	#endregion

	#endregion

	#region Flatten All Subfolders

	#region Only Reverse Geocode - Address

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> FlattenAllSubFoldersWithAddressNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.Address)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> FlattenAllSubFoldersWithAddressNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.Address)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> FlattenAllSubFoldersWithAddressNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.Address)
		}
	};

	#endregion

	#region Combine Photo Taken Date & Reverse Geocode - Address

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> FlattenAllSubFoldersWithDayAddressNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.DayAddress, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersItalyFlorence(),
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
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2),
			BuildRegex(PathCheckRegexType.DayAddress)
		}
	};

	#endregion

	#region Group By Folder

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> FlattenAllSubFoldersAndGroupByAddressFlatWithDayNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, isDryRun: false, groupByFolderType: GroupByFolderType.AddressFlat,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersUnitedKingdom(),
				SubFoldersKenya(),
				SubFoldersSpain1(),
				SubFoldersSpain2(),
				SubFoldersItalyFlorence(),
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
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoGpsCoordinate(),
				SubFoldersNoPhotoTakenDate(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 5),
			BuildRegex(PathCheckRegexType.GroupByAddressFolderNamingByDay, NoGpsCoordinateDayFormatFileName, NoPhotoTakenDateFileName, NoGpsCoordinateAndNoPhotoTakenDateFileName)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> FlattenAllSubFoldersAndGroupByAddressHierarchyWithDayNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.FlattenAllSubFolders,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue, groupByFolderType: GroupByFolderType.AddressHierarchy,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersUnitedKingdom(),
				SubFoldersKenya(),
				SubFoldersSpain1(),
				SubFoldersSpain2(),
				SubFoldersItalyFlorence(),
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
				SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
				SubFoldersNoGpsCoordinate(),
				SubFoldersNoPhotoTakenDate(),
			},
			new ConsoleOutputValues(18, 18, 15, 1, 2, 5),
			BuildRegex(PathCheckRegexType.GroupByAddressFolderNamingByDay, NoGpsCoordinateDayFormatFileName, NoPhotoTakenDateFileName, NoGpsCoordinateAndNoPhotoTakenDateFileName)
		}
	};

	#endregion

	#endregion

	#region Subfolders Preserve Folder Hierarchy

	#region Only Reverse Geocode - Address

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingOnlySequentialNumbers = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersUnitedKingdom(),
				SubFoldersItalyFlorence(),
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
				SubFoldersSpain1(),
				SubFoldersSpain2(),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3),
			BuildRegex(PathCheckRegexType.SubFolderAddressWithNumberNaming)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingPaddingZeroCharacter = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.PaddingZeroCharacter, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersUnitedKingdom(),
				SubFoldersItalyFlorence(),
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
				SubFoldersSpain1(),
				SubFoldersSpain2(),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3),
			BuildRegex(PathCheckRegexType.SubFolderAddressWithNumberNaming)
		}
	};

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingAllNamesAreSameLength = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Address, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.AllNamesAreSameLength, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud,
				bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersUnitedKingdom(),
				SubFoldersItalyFlorence(),
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
				SubFoldersSpain1(),
				SubFoldersSpain2(),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3),
			BuildRegex(PathCheckRegexType.SubFolderAddressWithNumberNaming)
		}
	};

	#endregion

	#region Combine Photo Taken Date & Reverse Geocode - Address

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string> SubFoldersPreserveFolderHierarchyWithDayAddressNaming = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.DayAddress, FolderProcessType.SubFoldersPreserveFolderHierarchy,
				NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.DontCopyToOutput, CopyNoCoordinateAction.DontCopyToOutput,
				reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
			new List<PhotoCsv>
			{
				SubFoldersKenya(),
				SubFoldersUnitedKingdom(),
				SubFoldersItalyFlorence(),
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
				SubFoldersSpain1(),
				SubFoldersSpain2(),
			},
			new ConsoleOutputValues(18, 15, 15, 1, 2, 3),
			BuildRegex(PathCheckRegexType.SubFolderDayAddressWithNumberNaming)
		}
	};

	#endregion

	#region Append Folder Name

	public static TheoryData<ICollection<string>, List<PhotoCsv>, ConsoleOutputValues, string>
		SubFoldersPreserveFolderHierarchyByAppendingMatchingMinimumAddressAsSuffixToFolderNameWithNumericNamingUsingOnlySequentialNumbers = new()
		{
			{
				CommandLineArgumentsFakes.CopyBuildCommandLineOptions(TestImagesPathHelper.SubFolders(), NamingStyle.Day, FolderProcessType.SubFoldersPreserveFolderHierarchy,
					NumberNamingTextStyle.OnlySequentialNumbers, CopyNoPhotoTakenDateAction.Continue, CopyNoCoordinateAction.Continue,
					folderAppendType: FolderAppendType.MatchingMinimumAddress, folderAppendLocationType: FolderAppendLocationType.Suffix,
					reverseGeocodeProvider: ReverseGeocodeProvider.BigDataCloud, bigDataCloudAdminLevels: new List<string> { "3", "4", "5", "6", "7" }),
				new List<PhotoCsv>
				{
					SubFoldersKenya(),
					SubFoldersUnitedKingdom(),
					SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(),
					SubFoldersSpain1(),
					SubFoldersSpain2(),
					SubFoldersNoPhotoTakenDate(),
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
				},
				new ConsoleOutputValues(18, 18, 15, 1, 2, 3),
				BuildRegex(PathCheckRegexType.SubFolderDayWithNumberNaming, AnyFolderLevelRegex + NoGpsCoordinateAndNoPhotoTakenDateFileName, AnyFolderLevelRegex + NoPhotoTakenDateFileName)
			}
		};

	#endregion

	#endregion

	[Theory]
	[MemberData(nameof(SingleFolderWithAddressNamingAndDuplicateNewNamesUsingOnlySequentialNumbers))]
	[MemberData(nameof(SingleFolderWithAddressNamingAndDuplicateNewNamesUsingPaddingZeroCharacter))]
	[MemberData(nameof(SingleFolderWithAddressNamingAndDuplicateNewNamesUsingAllNamesAreSameLength))]
	[MemberData(nameof(SingleFolderWithDateTimeWithDayAddressNaming))]
	[MemberData(nameof(FlattenAllSubFoldersWithAddressNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(FlattenAllSubFoldersWithAddressNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(FlattenAllSubFoldersWithAddressNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(FlattenAllSubFoldersWithDayAddressNaming))]
	[MemberData(nameof(FlattenAllSubFoldersAndGroupByAddressFlatWithDayNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(FlattenAllSubFoldersAndGroupByAddressHierarchyWithDayNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingOnlySequentialNumbers))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingPaddingZeroCharacter))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithWithAddressNamingUsingAllNamesAreSameLength))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyWithDayAddressNaming))]
	[MemberData(nameof(SubFoldersPreserveFolderHierarchyByAppendingMatchingMinimumAddressAsSuffixToFolderNameWithNumericNamingUsingOnlySequentialNumbers))]
	public async Task Running_With_Copy_Verb_Arguments_Should_Create_And_Verify_Photos_And_Report_Csv_On_File_System(ICollection<string> args, List<PhotoCsv> expectedPhotoCsvModels,
		ConsoleOutputValues expectedConsoleOutput, string regex)
	{

		var outputFolder = OutputFolderForE2ETestPrivateToEachTest();
		CommandLineArgumentsFakes.AddOutputPathOptions(outputFolder, args);

		var csvReportFile = new FileInfo(Path.Combine(outputFolder, ToolOptionFakes.CsvReportFileName));
		var (actualConsoleOutput, actualPhotoCsvModels) = await ExecuteCopy(args, csvReportFile);

		using (new AssertionScope())
		{
			actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels, c => c
				.Excluding(e => e.ReverseGeocodeFormatted).Excluding(e => e.PhotoNewPath)
				.Excluding(e => e.Address1).Excluding(e => e.Address2).Excluding(e => e.Address3).Excluding(e => e.Address4)
				.Excluding(e => e.Address5).Excluding(e => e.Address6).Excluding(e => e.Address7).Excluding(e => e.Address8));

			foreach (var actualPhotoCsvModel in actualPhotoCsvModels)
				actualPhotoCsvModel.PhotoNewPath.Should().MatchRegex(regex);

			actualConsoleOutput.Should().Be(expectedConsoleOutput);

			foreach (var newPhoto in actualPhotoCsvModels.Select(actualPhotoCsvModel => new FileInfo(actualPhotoCsvModel.PhotoNewPath!)))
				newPhoto.Exists.Should().Be(true);
		}

		DeleteOutput(outputFolder);
	}

	#region Utils

	private const string AnyFolderLevelRegex = @"([\w\s\/\-]*\/)?";

	private static string BuildRegex(PathCheckRegexType pathCheckRegexType, params string[] otherPossibleRegexToMatch)
	{
		string regex;
		switch (pathCheckRegexType)
		{
			case PathCheckRegexType.Address:
				regex = "(.*)+";
				break;
			case PathCheckRegexType.DayAddress:
				regex = @"\d{4}\.\d{2}\.\d{2}-(.*)+";
				break;
			case PathCheckRegexType.GroupByAddressFolderNamingByDay:
				regex = @"([\w\s\/\-]*)\/\d{4}\.\d{2}\.\d{2}(-\d+)?";
				break;
			case PathCheckRegexType.SubFolderAddressWithNumberNaming:
				regex = @"([\w\s\/\-]*)(-\d+)?";
				break;
			case PathCheckRegexType.SubFolderDayWithNumberNaming:
				regex = @"([\w\s\/\-]*\/)?\d{4}\.\d{2}\.\d{2}(-\d+)?";
				break;
			case PathCheckRegexType.SubFolderDayAddressWithNumberNaming:
				regex = @"([\w\s\/\-]*\/)?\d{4}\.\d{2}\.\d{2}([\w\s\/\-]*)(-\d+)?";
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(pathCheckRegexType), pathCheckRegexType, null);
		}

		if (otherPossibleRegexToMatch.Length > 0)
		{
			var conditions = new List<string> { regex };
			conditions.AddRange(otherPossibleRegexToMatch);
			regex = $"({string.Join("|", conditions)})";
		}
		return @"\/" + regex + @"\.jpg$";
	}

	#endregion
}
