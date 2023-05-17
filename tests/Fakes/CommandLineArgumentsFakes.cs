namespace PhotoCli.Tests.Fakes;

public static class CommandLineArgumentsFakes
{
	public static string[] SplitCommandLineArgs(string commandLine)
	{
		if (commandLine == null)
			throw new ArgumentNullException(nameof(commandLine));
		return commandLine.Split(" ");
	}


	public static string[] CopyBuildCommandLineOptions(string outputPath, string sourcePhotoPath, NamingStyle namingStyle, FolderProcessType folderProcessType,
		NumberNamingTextStyle numberNamingTextStyle, CopyNoPhotoTakenDateAction noPhotoTakenDateAction, CopyNoCoordinateAction noCoordinateAction, bool isDryRun = false,
		GroupByFolderType? groupByFolderType = null, FolderAppendType? folderAppendType = null, FolderAppendLocationType? folderAppendLocationType = null,
		ReverseGeocodeProvider? reverseGeocodeProvider = null, List<string>? bigDataCloudAdminLevels = null)
	{
		var args = new List<string> { "copy" };

		AddArgumentWithParameter('o', outputPath, args);
		AddArgumentWithParameter('s', namingStyle.ToString(), args);
		AddArgumentWithParameter('f', folderProcessType.ToString(), args);
		AddArgumentWithParameter('n', numberNamingTextStyle.ToString(), args);
		AddArgumentWithParameter('i', sourcePhotoPath, args);
		AddArgumentWithParameter('t', noPhotoTakenDateAction.ToString(), args);
		AddArgumentWithParameter('c', noCoordinateAction.ToString(), args);

		if (isDryRun)
			AddArgumentWithoutParameter('d', args);

		if (reverseGeocodeProvider != null)
			AddArgumentWithParameter('e', reverseGeocodeProvider.Value.ToString(), args);

		if (bigDataCloudAdminLevels != null)
			AddArgumentListWithParameter('u', bigDataCloudAdminLevels, args);

		if (groupByFolderType != null)
			AddArgumentWithParameter('g', groupByFolderType.Value.ToString(), args);

		if (folderAppendType != null)
			AddArgumentWithParameter('a', folderAppendType.Value.ToString(), args);

		if (folderAppendLocationType != null)
			AddArgumentWithParameter('p', folderAppendLocationType.Value.ToString(), args);

		return args.ToArray();
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

		AddArgumentWithParameter('o', outputReportFile, args);
		AddArgumentWithParameter('i', sourcePhotoPath, args);
		AddArgumentWithParameter('t', noPhotoTakenDateAction.ToString(), args);
		AddArgumentWithParameter('c', noCoordinateAction.ToString(), args);

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

		AddArgumentWithParameter('i', inputPath, args);
		AddArgumentWithParameter('t', addressListType.ToString(), args);
		AddArgumentWithParameter('e', reverseGeocodeProvider.ToString(), args);

		if (bigDataCloudAdminLevels != null)
		{
			AddArgumentWithoutParameter('u', args);
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
}
