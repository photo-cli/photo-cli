namespace PhotoCli.Tests.Fakes;

public static class CommandLineArgumentsFakes
{
	private const char OutputPathOptionNameShort = 'o';
	private const char InputPathOptionNameShort = 'i';
	private const char IsDryRunOptionNameShort = 'd';
	private const char ReverseGeocodeProvidersOptionNameShort = 'e';
	private const char NoPhotoDateTimeTakenActionOptionNameShort = 't';
	private const char NoCoordinateActionOptionNameShort = 'c';
	private const char InvalidFormatActionOptionNameShort = 'x';
	private const char BigDataCloudAdminLevelsOptionNameShort = 'u';

	public static string[] SplitCommandLineArgs(string commandLine)
	{
		if (commandLine == null)
			throw new ArgumentNullException(nameof(commandLine));
		return commandLine.Split(" ");
	}

	public static ICollection<string> CopyBuildCommandLineOptions(string sourcePhotoPath, NamingStyle namingStyle, FolderProcessType folderProcessType,
		NumberNamingTextStyle numberNamingTextStyle, CopyNoPhotoTakenDateAction noPhotoTakenDateAction, CopyNoCoordinateAction noCoordinateAction,
		string? outputPath = null, bool isDryRun = false, GroupByFolderType? groupByFolderType = null, FolderAppendType? folderAppendType = null, FolderAppendLocationType? folderAppendLocationType = null,
		ReverseGeocodeProvider? reverseGeocodeProvider = null, List<string>? bigDataCloudAdminLevels = null, CopyInvalidFormatAction invalidFormatAction = CopyInvalidFormatAction.PreventProcess)
	{
		var args = new List<string> { "copy" };

		AddArgumentWithParameter('s', namingStyle.ToString(), args);
		AddArgumentWithParameter('f', folderProcessType.ToString(), args);
		AddArgumentWithParameter('n', numberNamingTextStyle.ToString(), args);
		AddArgumentWithParameter(InputPathOptionNameShort, sourcePhotoPath, args);
		AddArgumentWithParameter(NoPhotoDateTimeTakenActionOptionNameShort, noPhotoTakenDateAction.ToString(), args);
		AddArgumentWithParameter(NoCoordinateActionOptionNameShort, noCoordinateAction.ToString(), args);
		AddArgumentWithParameter(InvalidFormatActionOptionNameShort, invalidFormatAction.ToString(), args);

		if(outputPath != null)
			AddArgumentWithParameter(OutputPathOptionNameShort, outputPath, args);

		if(isDryRun)
			AddArgumentWithoutParameter(IsDryRunOptionNameShort, args);

		if (reverseGeocodeProvider != null)
			AddArgumentWithParameter(ReverseGeocodeProvidersOptionNameShort, reverseGeocodeProvider.Value.ToString(), args);

		if (bigDataCloudAdminLevels != null)
			AddArgumentListWithParameter(BigDataCloudAdminLevelsOptionNameShort, bigDataCloudAdminLevels, args);

		if (groupByFolderType != null)
			AddArgumentWithParameter('g', groupByFolderType.Value.ToString(), args);

		if (folderAppendType != null)
			AddArgumentWithParameter('a', folderAppendType.Value.ToString(), args);

		if (folderAppendLocationType != null)
			AddArgumentWithParameter('p', folderAppendLocationType.Value.ToString(), args);

		return args;
	}

	private static void AddArgumentWithParameter(char optionNameShort, string value, ICollection<string> args)
	{
		args.Add($"-{optionNameShort}");
		args.Add(value);
	}

	private static void AddArgumentListWithParameter(char optionNameShort, List<string> argList, ICollection<string> args)
	{
		args.Add($"-{optionNameShort}");
		foreach (var argsValue in argList)
			args.Add(argsValue);
	}

	private static void AddArgumentWithoutParameter(char optionNameShort, ICollection<string> args)
	{
		args.Add($"-{optionNameShort}");
	}

	public static string[] InfoBuildCommandLineOptions(string outputReportFile, string sourcePhotoPath, bool allFolders, InfoNoPhotoTakenDateAction noPhotoTakenDateAction,
		InfoNoCoordinateAction noCoordinateAction, bool isDryRun, ReverseGeocodeProvider? reverseGeocodeProvider = null, List<string>? bigDataCloudAdminLevels = null)
	{
		var args = new List<string> { "info" };

		AddArgumentWithParameter(OutputPathOptionNameShort, outputReportFile, args);
		AddArgumentWithParameter(InputPathOptionNameShort, sourcePhotoPath, args);
		AddArgumentWithParameter(NoPhotoDateTimeTakenActionOptionNameShort, noPhotoTakenDateAction.ToString(), args);
		AddArgumentWithParameter(NoCoordinateActionOptionNameShort, noCoordinateAction.ToString(), args);

		if (isDryRun)
			AddArgumentWithoutParameter('d', args);

		if (allFolders)
			AddArgumentWithoutParameter('a', args);

		if (reverseGeocodeProvider != null)
			AddArgumentWithParameter('e', reverseGeocodeProvider.Value.ToString(), args);

		if (bigDataCloudAdminLevels != null)
			AddArgumentListWithParameter('u', bigDataCloudAdminLevels, args);

		return args.ToArray();
	}

	public static string[] AddressBuildCommandLineOptions(string inputPath, AddressListType addressListType, ReverseGeocodeProvider reverseGeocodeProvider,
		IEnumerable<int>? bigDataCloudAdminLevels = null)
	{
		var args = new List<string> { "address" };

		AddArgumentWithParameter(InputPathOptionNameShort, inputPath, args);
		AddArgumentWithParameter('t', addressListType.ToString(), args);
		AddArgumentWithParameter(ReverseGeocodeProvidersOptionNameShort, reverseGeocodeProvider.ToString(), args);

		if (bigDataCloudAdminLevels != null)
		{
			AddArgumentWithoutParameter(BigDataCloudAdminLevelsOptionNameShort, args);
			foreach (var bigDataCloudAdminLevel in bigDataCloudAdminLevels)
				args.Add(bigDataCloudAdminLevel.ToString());
		}

		return args.ToArray();
	}

	public static string[] SettingsBuildCommandLineOptions(string? key = null, string? value = null, bool reset = false)
	{
		var args = new List<string> { "settings" };

		if (key != null)
			AddArgumentWithParameter('k', key, args);

		if (value != null)
			AddArgumentWithParameter('v', value, args);

		if (reset)
			AddArgumentWithoutParameter('r', args);

		return args.ToArray();
	}

	public static string[] CopyOptionsInvalid()
	{
		var args = new[] { "copy", "-o", "output-folder" };
		return args;
	}

	public static string[] InfoOptionsInvalid()
	{
		var args = new[] { "info" };
		return args;
	}

	public static string[] AddressOptionsInvalid()
	{
		var args = new[] { "address" };
		return args;
	}

	public static string[] SettingsOptionsInvalid()
	{
		var args = new[] { "settings", "--value", "value1" };
		return args;
	}

	public static string[] NotParseable()
	{
		var args = new[] { "--invalid-option value" };
		return args;
	}

	public static ICollection<string> ArchiveBuildCommandLineOptionsWithoutOutputPath(string sourcePhotoPath, bool isDryRun = false,
		ArchiveInvalidFormatAction invalidFormatAction = ArchiveInvalidFormatAction.Continue, ReverseGeocodeProvider? reverseGeocodeProvider = null, List<string>? bigDataCloudAdminLevels = null,
		ArchiveNoPhotoTakenDateAction noPhotoTakenDateAction = ArchiveNoPhotoTakenDateAction.Continue, ArchiveNoCoordinateAction noCoordinateAction = ArchiveNoCoordinateAction.Continue)
	{
		var args = new List<string> { "archive" };

		AddArgumentWithParameter(InputPathOptionNameShort, sourcePhotoPath, args);
		if(isDryRun)
			AddArgumentWithoutParameter(IsDryRunOptionNameShort, args);
		AddArgumentWithParameter(InvalidFormatActionOptionNameShort, invalidFormatAction.ToString(), args);
		AddArgumentWithParameter(NoPhotoDateTimeTakenActionOptionNameShort, noPhotoTakenDateAction.ToString(), args);
		AddArgumentWithParameter(NoCoordinateActionOptionNameShort, noCoordinateAction.ToString(), args);

		if (reverseGeocodeProvider != null)
			AddArgumentWithParameter(ReverseGeocodeProvidersOptionNameShort, reverseGeocodeProvider.Value.ToString(), args);

		if (bigDataCloudAdminLevels != null)
			AddArgumentListWithParameter(BigDataCloudAdminLevelsOptionNameShort, bigDataCloudAdminLevels, args);

		return args;
	}

	public static void AddOutputPathOptions(string outputPath, ICollection<string> args)
	{
		AddArgumentWithParameter(OutputPathOptionNameShort, outputPath, args);
	}
}
