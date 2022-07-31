namespace PhotoCli.Tests.Fakes.Options;

public static class CopyOptionsFakes
{
	private const string ValidOutputPath = "output-folder";

	public static CopyOptions WithPaths(string outputFolderPath, string? sourceFolderPath = null)
	{
		return Create(outputFolderPath, sourcePhotosFolderPathOptional: sourceFolderPath);
	}

	public static CopyOptions WithNoExifDataAction(string sourceFolderPath, CopyNoPhotoTakenDateAction noPhotoTakenDateAction, CopyNoCoordinateAction noCoordinateAction)
	{
		return Create(sourcePhotosFolderPathOptional: sourceFolderPath, noPhotoTakenDateActionOptional: noPhotoTakenDateAction,
			noCoordinateActionOptional: noCoordinateAction);
	}

	public static CopyOptions WithFolderProcessTypeSingle(GroupByFolderType? groupByFolderType = null)
	{
		return Create(folderProcessTypeRequired: FolderProcessType.Single, groupByFolderTypeOptional: groupByFolderType);
	}

	public static CopyOptions WithFolderProcessTypeFlattenAllSubFolders(GroupByFolderType? groupByFolderType = null)
	{
		return Create(folderProcessTypeRequired: FolderProcessType.FlattenAllSubFolders, groupByFolderTypeOptional: groupByFolderType);
	}

	public static CopyOptions WithFolderProcessTypeSubFoldersPreserveFolderHierarchy(FolderAppendType? folderAppendType = null,
		FolderAppendLocationType? folderAppendLocationType = null)
	{
		return Create(folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy, folderAppendTypeOptional: folderAppendType,
			folderAppendLocationTypeOptional: folderAppendLocationType);
	}

	public static CopyOptions WithFolderAppendType(string outputFolderPath, string sourceFolderPath)
	{
		return Create(outputFolderPath, folderProcessTypeRequired: FolderProcessType.SubFoldersPreserveFolderHierarchy, folderAppendTypeOptional: FolderAppendTypeFakes.Valid(),
			folderAppendLocationTypeOptional: FolderAppendLocationTypeFakes.Valid(), sourcePhotosFolderPathOptional: sourceFolderPath);
	}

	public static CopyOptions WithoutFolderAppendType(string outputFolderPath, string sourceFolderPath)
	{
		return Create(outputFolderPath, sourcePhotosFolderPathOptional: sourceFolderPath);
	}

	public static CopyOptions WithReverseGeocode(string outputFolderPath, string sourceFolderPath)
	{
		return Create(outputFolderPath, sourcePhotosFolderPathOptional: sourceFolderPath, reverseGeoCodeProviderOptional: ReverseGeocodeProvider.BigDataCloud,
			bigDataCloudAdminLevelsOptional: new List<int> { 2, 4, 6, 8 });
	}

	public static CopyOptions WithDryRun(string outputFolderPath, string sourceFolderPath)
	{
		return Create(outputFolderPath, sourcePhotosFolderPathOptional: sourceFolderPath, isDryRunOptional: true);
	}

	public static CopyOptions Valid()
	{
		return Create();
	}

	public static CopyOptions WithReverseGeocodeService(ReverseGeocodeProvider reverseGeocodeProvider = ReverseGeocodeProvider.BigDataCloud)
	{
		return Create(reverseGeoCodeProviderOptional: reverseGeocodeProvider);
	}

	public static CopyOptions Create(
		// required
		string? outputPath = ValidOutputPath, NamingStyle? namingStyleRequired = null, FolderProcessType? folderProcessTypeRequired = null,
		NumberNamingTextStyle? numberNamingTextStyleRequired = null, CopyNoPhotoTakenDateAction? noPhotoTakenDateActionOptional = null, CopyNoCoordinateAction? noCoordinateActionOptional = null,
		// optional
		string? sourcePhotosFolderPathOptional = null, bool isDryRunOptional = false, GroupByFolderType? groupByFolderTypeOptional = null,
		FolderAppendType? folderAppendTypeOptional = null, FolderAppendLocationType? folderAppendLocationTypeOptional = null,
		// shared ReverseGeocode
		ReverseGeocodeProvider reverseGeoCodeProviderOptional = ReverseGeocodeProvider.Disabled,
		IEnumerable<int>? bigDataCloudAdminLevelsOptional = null)
	{
		return new CopyOptions(outputPath!, namingStyleRequired ?? NamingStyleFakes.Valid(),
			folderProcessTypeRequired ?? FolderProcessTypeFakes.Valid(), numberNamingTextStyleRequired ?? NumberNamingTextStyleFakes.Valid(),
			noPhotoTakenDateActionOptional ?? CopyNoPhotoTakenDateActionFakes.Valid(), noCoordinateActionOptional ?? CopyNoCoordinateActionFakes.Valid(), sourcePhotosFolderPathOptional,
			isDryRunOptional, groupByFolderTypeOptional, folderAppendTypeOptional, folderAppendLocationTypeOptional, reverseGeoCodeProviderOptional,
			bigDataCloudAdminLevels: bigDataCloudAdminLevelsOptional);
	}

	public static CopyOptions ValidReverseGeocodeService(ReverseGeocodeProvider reverseGeocodeProvider = ReverseGeocodeProvider.BigDataCloud)
	{
		return new CopyOptions(ValidOutputPath, NamingStyleFakes.Valid(), FolderProcessTypeFakes.Valid(), NumberNamingTextStyleFakes.Valid(), CopyNoPhotoTakenDateActionFakes.Valid(),
			CopyNoCoordinateActionFakes.Valid(), reverseGeoCodeProvider: reverseGeocodeProvider);
	}

	public static CopyOptions ValidReverseGeocodeServiceWithLicense(bool hasPaidLicense, ReverseGeocodeProvider reverseGeocodeProvider = ReverseGeocodeProvider.BigDataCloud)
	{
		return new CopyOptions(ValidOutputPath, NamingStyleFakes.Valid(), FolderProcessTypeFakes.Valid(), NumberNamingTextStyleFakes.Valid(), CopyNoPhotoTakenDateActionFakes.Valid(),
			CopyNoCoordinateActionFakes.Valid(), reverseGeoCodeProvider: reverseGeocodeProvider, hasPaidLicense: hasPaidLicense);
	}
}
