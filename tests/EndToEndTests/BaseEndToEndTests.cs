namespace PhotoCli.Tests.EndToEndTests;

public abstract class BaseEndToEndTests : IClassFixture<SetEnvironmentVariablesFromLaunchSettingsFixture>
{
	private readonly ITestOutputHelper _testOutputHelper;

	protected BaseEndToEndTests(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	#region Single Folder PhotoCsv

	protected static PhotoCsv SingleKenya(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Kenya.jpg"), ExifDataFakes.Kenya());
	}

	protected static PhotoCsv SingleItalyFlorence(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Florence.jpg"), ExifDataFakes.ItalyFlorence());
	}

	protected static PhotoCsv SingleItalyArezzo1(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-1.jpg"), ExifDataFakes.ItalyArezzo1());
	}

	protected static PhotoCsv SingleItalyArezzo2(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-2.jpg"), ExifDataFakes.ItalyArezzo2());
	}

	protected static PhotoCsv SingleItalyArezzo3(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-3.jpg"), ExifDataFakes.ItalyArezzo3());
	}

	protected static PhotoCsv SingleItalyArezzo4(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-4.jpg"), ExifDataFakes.ItalyArezzo4());
	}

	protected static PhotoCsv SingleItalyArezzo5(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-5.jpg"), ExifDataFakes.ItalyArezzo5());
	}

	protected static PhotoCsv SingleItalyArezzo6(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-6.jpg"), ExifDataFakes.ItalyArezzo6());
	}

	protected static PhotoCsv SingleItalyArezzo7(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-7.jpg"), ExifDataFakes.ItalyArezzo7());
	}

	protected static PhotoCsv SingleItalyArezzo8(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-8.jpg"), ExifDataFakes.ItalyArezzo8());
	}

	protected static PhotoCsv SingleItalyArezzo9(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-9.jpg"), ExifDataFakes.ItalyArezzo9());
	}

	protected static PhotoCsv SingleItalyArezzo9Duplicate(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Italy-Arezzo-9-duplicate.jpg"), ExifDataFakes.ItalyArezzo9());
	}

	protected static PhotoCsv SingleUnitedKingdom(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("United-Kingdom.jpg"), ExifDataFakes.UnitedKingdom());
	}

	protected static PhotoCsv SingleSpain1(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Spain-1.jpg"), ExifDataFakes.Spain1());
	}

	protected static PhotoCsv SingleSpain2(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("Spain-2.jpg"), ExifDataFakes.Spain2());
	}

	protected static PhotoCsv SingleNoPhotoTakenDate(string? newFileName = null)
	{
		return PhotoCsvWithoutExifDataJpg(newFileName, TestImagesPathHelper.SingleFolder("NoPhotoTakenDate.jpg"));
	}

	protected static PhotoCsv SingleNoPhotoTakenDateWithDefaultName(bool useFullPath = true)
	{
		return SingleNoPhotoTakenDate(NoPhotoTakenDateFileName);
	}

	protected static PhotoCsv SingleNoGpsCoordinate(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolder("NoGpsCoordinate.jpg"), ExifDataFakes.NoGpsCoordinate());
	}

	protected const string NoGpsCoordinateAndNoPhotoTakenDateFileName = "NoGpsCoordinateAndNoPhotoTakenDate";

	protected static PhotoCsv SingleNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName()
	{
		return SingleNoGpsCoordinateAndNoPhotoTakenDate(NoGpsCoordinateAndNoPhotoTakenDateFileName);
	}

	protected static PhotoCsv SingleNoGpsCoordinateAndNoPhotoTakenDate(string? newFileName = null)
	{
		return PhotoCsvWithoutExifDataJpg(newFileName, TestImagesPathHelper.SingleFolder("NoGpsCoordinateAndNoPhotoTakenDate.jpg"));
	}

	#endregion

	#region Sub Folders PhotoCsv

	protected static PhotoCsv SubFoldersKenya(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("Kenya.jpg"), ExifDataFakes.Kenya());
	}

	protected static PhotoCsv SubFoldersItalyFlorence(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Florence/Italy-Florence.jpg"), ExifDataFakes.ItalyFlorence());
	}

	protected static PhotoCsv SubFoldersItalyArezzo1(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-1.jpg"), ExifDataFakes.ItalyArezzo1());
	}

	protected static PhotoCsv SubFoldersItalyArezzo2(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-2.jpg"), ExifDataFakes.ItalyArezzo2());
	}

	protected static PhotoCsv SubFoldersItalyArezzo3(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-3.jpg"), ExifDataFakes.ItalyArezzo3());
	}

	protected static PhotoCsv SubFoldersItalyArezzo4(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-4.jpg"), ExifDataFakes.ItalyArezzo4());
	}

	protected static PhotoCsv SubFoldersItalyArezzo5(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-5.jpg"), ExifDataFakes.ItalyArezzo5());
	}

	protected static PhotoCsv SubFoldersItalyArezzo6(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-6.jpg"), ExifDataFakes.ItalyArezzo6());
	}

	protected static PhotoCsv SubFoldersItalyArezzo7(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-7.jpg"), ExifDataFakes.ItalyArezzo7());
	}

	protected static PhotoCsv SubFoldersItalyArezzo8(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-8.jpg"), ExifDataFakes.ItalyArezzo8());
	}

	protected static PhotoCsv SubFoldersItalyArezzo9(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-9.jpg"), ExifDataFakes.ItalyArezzo9());
	}

	protected static PhotoCsv SubFoldersItalyArezzo9Duplicate(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Arezzo/Italy-Arezzo-9-duplicate.jpg"), ExifDataFakes.ItalyArezzo9Duplicate());
	}

	protected static PhotoCsv SubFoldersUnitedKingdom(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("United-Kingdom.jpg"), ExifDataFakes.UnitedKingdom());
	}

	protected static PhotoCsv SubFoldersSpain1(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("SpainFolder/Spain-1.jpg"), ExifDataFakes.Spain1());
	}

	protected static PhotoCsv SubFoldersSpain2(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("SpainFolder/Spain-2.jpg"), ExifDataFakes.Spain2());
	}

	private static readonly DateTime NoGpsCoordinatePhotoTakenDateTime = new(2008, 7, 16, 11, 33, 20);
	protected static readonly string NoGpsCoordinateDayFormatFileName = NoGpsCoordinatePhotoTakenDateTime.ToString("yyyy.MM.dd");
	protected static PhotoCsv SubFoldersNoGpsCoordinate(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFolders("ItalyFolder/Florence/NoGpsCoordinate.jpg"), ExifDataFakes.WithPhotoTakenDate(NoGpsCoordinatePhotoTakenDateTime));
	}

	protected const string NoPhotoTakenDateFileName = "NoPhotoTakenDate";

	protected static PhotoCsv SubFoldersNoPhotoTakenDateWithDefaultName()
	{
		return SubFoldersNoPhotoTakenDate(NoPhotoTakenDateFileName);
	}

	protected static PhotoCsv SubFoldersNoPhotoTakenDate(string? newFileName = null)
	{
		return PhotoCsvWithoutExifDataJpg(newFileName, TestImagesPathHelper.SubFolders("SpainFolder/NoPhotoTakenDate.jpg"));
	}

	protected static PhotoCsv SubFoldersNoGpsCoordinateAndNoPhotoTakenDateWithDefaultName()
	{
		return SubFoldersNoGpsCoordinateAndNoPhotoTakenDate(NoGpsCoordinateAndNoPhotoTakenDateFileName);
	}

	protected static PhotoCsv SubFoldersNoGpsCoordinateAndNoPhotoTakenDate(string? newFileName = null)
	{
		return PhotoCsvWithoutExifDataJpg(newFileName, TestImagesPathHelper.SubFolders("NoGpsCoordinateAndNoPhotoTakenDate.jpg"));
	}

	#endregion

	#region Single Folder Companions PhotoCsv

	protected static PhotoCsv SingleCompanionsAmsterdam(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolderCompanions("amsterdam.HEIC"), ExifDataFakes.CompanionsAmsterdam());
	}

	protected static PhotoCsv SingleCompanionsChios(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolderCompanions("chios.HEIC"), ExifDataFakes.CompanionsChios());
	}

	protected static PhotoCsv SingleCompanionsCopenhagen(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolderCompanions("copenhagen.HEIC"), ExifDataFakes.CompanionsCopenhagen());
	}

	protected static PhotoCsv SingleCompanionsHallstatt(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolderCompanions("hallstatt.HEIC"), ExifDataFakes.CompanionsHallstatt());
	}

	protected static PhotoCsv SingleCompanionsLeiden(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SingleFolderCompanions("leiden.HEIC"), ExifDataFakes.CompanionsLeiden());
	}

	#endregion

	#region Sub Folders Companions PhotoCsv

	protected static PhotoCsv SubFoldersCompanionsAmsterdam(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFoldersCompanions("Netherlands/amsterdam.HEIC"), ExifDataFakes.CompanionsAmsterdam());
	}

	protected static PhotoCsv SubFoldersCompanionsChios(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFoldersCompanions("Summer/chios.HEIC"), ExifDataFakes.CompanionsChios());
	}

	protected static PhotoCsv SubFoldersCompanionsCopenhagen(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFoldersCompanions("Winter/copenhagen.HEIC"), ExifDataFakes.CompanionsCopenhagen());
	}

	protected static PhotoCsv SubFoldersCompanionsHallstatt(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFoldersCompanions("Winter/hallstatt.HEIC"), ExifDataFakes.CompanionsHallstatt());
	}

	protected static PhotoCsv SubFoldersCompanionsLeiden(string? newFileName = null)
	{
		return PhotoCsvWithExifData(newFileName, TestImagesPathHelper.SubFoldersCompanions("Netherlands/leiden.HEIC"), ExifDataFakes.CompanionsLeiden());
	}

	#endregion

	private static PhotoCsv PhotoCsvWithExifData(string? newFileName, string filePath, ExifData exifData)
	{
		var extension = GetFileNameExtension(filePath);
		var outputPhotoFile = OutputPhotoFile(newFileName, extension);
		var fullFilePath = FullPath(filePath);
		return PhotoCsvFakes.WithExifData(exifData, fullFilePath, outputPhotoFile, useRelativePathOutput: true);
	}

	private static PhotoCsv PhotoCsvWithoutExifDataJpg(string? newFileName, string filePath)
	{
		var extension = GetFileNameExtension(filePath);
		var outputPhotoFile = OutputPhotoFile(newFileName, extension);
		var fullFilePath = FullPath(filePath);
		return PhotoCsvFakes.CreateWithoutExifData(fullFilePath, outputPhotoFile, true);
	}

	protected ConsoleOutputValues ParseConsoleOutput(string actualOutput)
	{
		var photosFound = GetRegexValue(@"(.\d+) photo\(s\) found.", actualOutput);
		var photosCopied = GetRegexValue(@"(.\d+) photo\(s\) copied.", actualOutput);
		var hasTakenDateAndCoordinate = GetRegexValue(@"(.\d+) photo\(s\) has taken date and coordinate.", actualOutput);
		var hasTakenDateButNoCoordinate = GetRegexValue(@"(.\d+) photo\(s\) has taken date but no coordinate.", actualOutput);
		var hasNoTakenDateAndCoordinate = GetRegexValue(@"(.\d+) photo\(s\) has no taken date and coordinate.", actualOutput);
		var directoriesCreated = GetRegexValue(@"(.\d+) directory/directories created.", actualOutput);
		var companionsFound = GetRegexValue(@"(.\d+) companion file\(s\) found.", actualOutput);
		var companionsCopied = GetRegexValue(@"(.\d+) companion file\(s\) copied.", actualOutput);

		return new ConsoleOutputValues(photosFound, photosCopied, hasTakenDateAndCoordinate, hasTakenDateButNoCoordinate, hasNoTakenDateAndCoordinate,
			directoriesCreated, companionsFound, companionsCopied);
	}

	private int GetRegexValue(string regex, string actualOutput)
	{
		var value = new Regex(regex).Match(actualOutput).Groups[1].Value;
		return value.IsPresent() ? int.Parse(value) : 0;
	}

	private static string FullPath(string filePath)
	{
		return Path.Combine(AppContext.BaseDirectory, filePath);
	}

	protected Task<string> RunMain(IEnumerable<string> args, ExitCode expectedExitCode = ExitCode.Success)
	{
		return RunMain(args.ToArray(), expectedExitCode);
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
		var actualNormalized = actual.ReplaceLineEndings();
		var expectedNormalized = expected.ReplaceLineEndings();
		actualNormalized.ReplaceLineEndings().Should().Be(expectedNormalized);
	}

	protected static string OutputFolderForE2ETestPrivateToEachTest()
	{
		return Path.Combine("e2e-tests-output", Guid.NewGuid().ToString());
	}

	protected static void DeleteOutput(string outputPath)
	{
		if (Directory.Exists(outputPath))
			Directory.Delete(outputPath, true);
	}

	private static string? OutputPhotoFile(string? newFileName, string fileExtension)
	{
		return newFileName != null ? $"{newFileName}.{fileExtension}" : null;
	}

	private static string GetFileNameExtension(string filePath)
	{
		var lastIndexOfDot = filePath.LastIndexOf('.');
		return filePath[(lastIndexOfDot + 1)..].ToLower();
	}

	protected static void VerifyExpectedFilesOnOutput(List<string> expectedFiles, string outputFolder)
	{
		var fileInfos = expectedFiles.Select(s => new FileInfo(Path.Combine(outputFolder, s)));
		foreach (var fileInfo in fileInfos)
			fileInfo.Exists.Should().Be(true, $"File {fileInfo.FullName} must exists on file system");
	}
}
