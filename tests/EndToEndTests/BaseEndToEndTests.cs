namespace PhotoCli.Tests.EndToEndTests;

public abstract class BaseEndToEndTests : IClassFixture<SetEnvironmentVariablesFromLaunchSettingsFixture>
{
	private readonly ITestOutputHelper _testOutputHelper;
	private const string OutputPath = "output-folder";

	protected BaseEndToEndTests(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	#region Single Folder - PhotoCsv

	protected static PhotoCsv SingleKenya(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Kenya.jpg"), new DateTime(2005, 8, 13, 9, 47, 23), -0.37129999999999996, 36.056416666666664, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyFlorence(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Florence.jpg"), new DateTime(2005, 12, 14, 14, 39, 47), 43.78559443333333, 11.234619433333334, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo1(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-1.jpg"), new DateTime(2008, 10, 22, 16, 28, 39), 43.46744833333334, 11.885126666663888, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo2(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-2.jpg"), new DateTime(2008, 10, 22, 16, 29, 49), 43.46715666666389, 11.885394999997223, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo3(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-3.jpg"), new DateTime(2008, 10, 22, 16, 38, 20), 43.467081666663894, 11.884538333330555, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo4(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-4.jpg"), new DateTime(2008, 10, 22, 16, 43, 21), 43.468365, 11.881634999972222, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo5(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-5.jpg"), new DateTime(2008, 10, 22, 16, 44, 1), 43.46844166666667, 11.881515, useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo6(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-6.jpg"), new DateTime(2008, 10, 22, 16, 46, 53), 43.468243333330555, 11.880171666638889, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo7(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-7.jpg"), new DateTime(2008, 10, 22, 16, 52, 15), 43.467254999997223, 11.879213333333334, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo8(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-8.jpg"), new DateTime(2008, 10, 22, 16, 55, 37), 43.466011666638892, 11.87911166663889, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo9(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-9.jpg"), new DateTime(2008, 10, 22, 17, 0, 7), 43.464455000000001, 11.881478333333334, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleItalyArezzo9Duplicate(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-9-duplicate.jpg"), new DateTime(2008, 10, 22, 17, 0, 7), 43.464455000000001, 11.881478333333334, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleUnitedKingdom(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("United-Kingdom.jpg"), new DateTime(2012, 6, 22, 19, 52, 31), 51.424838333333334, -0.67356166666666661, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleSpain1(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Spain-1.jpg"), new DateTime(2015, 4, 10, 20, 12, 23), 40.446972222222222, -3.7247527777777778, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleSpain2(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("Spain-2.jpg"), new DateTime(2015, 4, 10, 20, 12, 23), 40.446972222222222, -3.7247527777777778, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SingleNoPhotoTakenDate(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("NoPhotoTakenDate.jpg"), useFullPath: useFullPath, reverseGeocodes: reverseGeocodes);
	}

	protected static PhotoCsv SingleNoPhotoTakenDateWithDefaultName(bool useFullPath = true)
	{
		return SingleNoPhotoTakenDate("NoPhotoTakenDate", useFullPath);
	}

	protected static PhotoCsv SingleNoGpsCoordinate(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("NoGpsCoordinate.jpg"), new DateTime(2008, 7, 16, 11, 33, 20), useFullPath: useFullPath, reverseGeocodes: reverseGeocodes);
	}

	protected static PhotoCsv SingleNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(bool useFullPath = true)
	{
		return SingleNoGpsCoordinateAndNoPhotoTakenDate("NoGpsCoordinateAndNoPhotoTakenDate", useFullPath);
	}

	protected static PhotoCsv SingleNoGpsCoordinateAndNoPhotoTakenDate(string? newFileName = null, bool useFullPath = true)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SingleFolder("NoGpsCoordinateAndNoPhotoTakenDate.jpg"), useFullPath: useFullPath);
	}

	#endregion

	#region Subfolder PhotoCsv

	protected static PhotoCsv SubFoldersKenya(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("Kenya.jpg"), new DateTime(2005, 8, 13, 9, 47, 23), -0.37129999999999996, 36.056416666666664, useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyFlorence(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Florence/Italy-Florence.jpg"), new DateTime(2005, 12, 14, 14, 39, 47), 43.78559443333333, 11.234619433333334,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo1(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-1.jpg"), new DateTime(2008, 10, 22, 16, 28, 39), 43.46744833333334, 11.885126666663888,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo2(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-2.jpg"), new DateTime(2008, 10, 22, 16, 29, 49), 43.46715666666389, 11.885394999997223,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo3(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-3.jpg"), new DateTime(2008, 10, 22, 16, 38, 20), 43.467081666663894, 11.884538333330555,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo4(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-4.jpg"), new DateTime(2008, 10, 22, 16, 43, 21), 43.468365, 11.881634999972222, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo5(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-5.jpg"), new DateTime(2008, 10, 22, 16, 44, 1), 43.46844166666667, 11.881515, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo6(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-6.jpg"), new DateTime(2008, 10, 22, 16, 46, 53), 43.468243333330555, 11.880171666638889,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo7(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-7.jpg"), new DateTime(2008, 10, 22, 16, 52, 15), 43.467254999997223, 11.879213333333334,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo8(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-8.jpg"), new DateTime(2008, 10, 22, 16, 55, 37), 43.466011666638892, 11.87911166663889,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo9(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-9.jpg"), new DateTime(2008, 10, 22, 17, 0, 7), 43.464455000000001, 11.881478333333334,
			useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersItalyArezzo9Duplicate(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-9-duplicate.jpg"), new DateTime(2008, 10, 22, 17, 0, 7), 43.464455000000001,
			11.881478333333334, useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersUnitedKingdom(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("United-Kingdom.jpg"), new DateTime(2012, 6, 22, 19, 52, 31), 51.424838333333334, -0.67356166666666661, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersSpain1(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("SpainFolder/Spain-1.jpg"), new DateTime(2015, 4, 10, 20, 12, 23), 40.446972222222222, -3.7247527777777778, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersSpain2(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("SpainFolder/Spain-2.jpg"), new DateTime(2015, 4, 10, 20, 12, 23), 40.446972222222222, -3.7247527777777778, useFullPath,
			reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersNoGpsCoordinateWithDefaultName(bool useFullPath = true, params string[] reverseGeocodes)
	{
		return SubFoldersNoGpsCoordinate("NoGpsCoordinate", useFullPath, reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersNoGpsCoordinate(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Florence/NoGpsCoordinate.jpg"), new DateTime(2008, 7, 16, 11, 33, 20), useFullPath: useFullPath,
			reverseGeocodes: reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersNoPhotoTakenDateWithDefaultName(bool useFullPath = true)
	{
		return SubFoldersNoPhotoTakenDate("NoPhotoTakenDate", useFullPath);
	}

	protected static PhotoCsv SubFoldersNoPhotoTakenDate(string? newFileName = null, bool useFullPath = true, params string[] reverseGeocodes)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("SpainFolder/NoPhotoTakenDate.jpg"), useFullPath: useFullPath, reverseGeocodes: reverseGeocodes);
	}

	protected static PhotoCsv SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName(bool useFullPath = true)
	{
		return SubFoldersNoGpsCoordinateAndNoPhotoTakenDate("NoGpsCoordinateAndNoPhotoTakenDate", useFullPath);
	}

	protected static PhotoCsv SubFoldersNoGpsCoordinateAndNoPhotoTakenDate(string? newFileName = null, bool useFullPath = true)
	{
		return CreatePhotoCsv(newFileName, TestImagesPathHelper.SubFolders("NoGpsCoordinateAndNoPhotoTakenDate.jpg"), useFullPath: useFullPath);
	}

	#endregion

	private static PhotoCsv CreatePhotoCsv(string? newFileName, string filePath, DateTime? takenDateTime = null, double? latitude = null, double? longitude = null, bool useFullPath = true,
		params string[] reverseGeocodes)
	{
		var outputPhotoPath = newFileName != null ? PathWithOutput($"{newFileName}.jpg") : null;
		if (useFullPath)
			filePath = FullPath(filePath);

		return PhotoCsvFakes.Create(filePath, outputPhotoPath, takenDateTime, reverseGeocodes.ToList(), latitude, longitude, true);
	}

	protected ConsoleOutputValues ParseConsoleOutput(string actualOutput)
	{
		var photosFound = GetRegexValue(@"(.\d+) photos found.", actualOutput);
		var photosCopied = GetRegexValue(@"(.\d+) photos copied.", actualOutput);
		var hasTakenDateAndCoordinate = GetRegexValue(@"(.\d+) photos has taken date and coordinate.", actualOutput);
		var hasTakenDateButNoCoordinate = GetRegexValue(@"(.\d+) photos has taken date but no coordinate.", actualOutput);
		var hasNoTakenDateAndCoordinate = GetRegexValue(@"(.\d+) photos has no taken date and coordinate.", actualOutput);
		var directoriesCreated = GetRegexValue(@"(.\d+) directories created.", actualOutput);
		return new ConsoleOutputValues(photosFound, photosCopied, hasTakenDateAndCoordinate, hasTakenDateButNoCoordinate, hasNoTakenDateAndCoordinate, directoriesCreated);
	}

	private int GetRegexValue(string regex, string actualOutput)
	{
		var value = new Regex(regex).Match(actualOutput).Groups[1].Value;
		return value.IsPresent() ? int.Parse(value) : 0;
	}

	private static string PathWithOutput(string fileName)
	{
		return Path.Combine(OutputPath, fileName);
	}

	private static string FullPath(string filePath)
	{
		return Path.Combine(AppContext.BaseDirectory, filePath);
	}

	protected async Task<string> RunMain(string[] args, ExitCode expectedExitCode = ExitCode.Success)
	{
		var stringWriter = new StringWriter();
		var exitCode = (ExitCode)await Program.MainStream(args, stringWriter);
		_testOutputHelper.WriteLine(stringWriter.ToString());
		var actualOutput = GetTrimmedConsoleOutput(stringWriter);
		exitCode.Should().Be(expectedExitCode);
		return actualOutput;
	}

	private string GetTrimmedConsoleOutput(StringWriter stringWriter)
	{
		var rawOutput = stringWriter.GetStringBuilder().ToString();
		var lines = rawOutput.Split(Environment.NewLine);
		var trimmedLines = lines.Select(s => s.Trim());
		var mergedLines = string.Join(Environment.NewLine, trimmedLines.SkipLast(1));
		return mergedLines;
	}

	protected void StringsShouldMatchDiscardingLineEndings(string actual, string expected)
	{
		actual.ReplaceLineEndings().Should().Be(expected.ReplaceLineEndings());
	}
}
