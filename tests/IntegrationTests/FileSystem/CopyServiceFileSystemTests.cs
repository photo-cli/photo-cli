namespace PhotoCli.Tests.IntegrationTests.FileSystem;

public class CopyServiceFileSystemTests
{
	private const string OutputPath = "/output";
	private const string SourcePath = "/source";

	#region Csv

	#region Photo Taken Date Data

	private static readonly DateTime Date2000 = DateTimeFakes.WithYear(2000);
	private static readonly DateTime Date2001 = DateTimeFakes.WithYear(2001);

	private static readonly ExifData ExifData2000 = ExifDataFakes.WithPhotoTakenDate(Date2000);
	private static readonly ExifData ExifData2001 = ExifDataFakes.WithPhotoTakenDate(Date2001);

	public static TheoryData<List<Photo>, List<PhotoCsv>> PhotoDateTakenData = new()
	{
		{
			[
				PhotoFakes.CreateWithExifData(ExifData2000, "photo0.jpg", sourcePath: SourcePath),
				PhotoFakes.CreateWithExifData(ExifData2001, "photo1.jpg", sourcePath: SourcePath)
			],
			[
				PhotoCsvFakes.CreateWithFileName(ExifData2000, "photo0.jpg", SourcePath),
				PhotoCsvFakes.CreateWithFileName(ExifData2001, "photo1.jpg", SourcePath)
			]
		}
	};

	#endregion

	#region Coordinate ReverseGeocode Data

	private const double Latitude0 = 0;
	private const double Longitude0 = 0;
	private static readonly List<string> ReverseGeocodesLatitude0Longitude0 = ReverseGeocodeFakes.WithCoordinate(Latitude0, Longitude0);

	private const double Latitude1 = 1;
	private const double Longitude1 = 1;
	private static readonly List<string> ReverseGeocodesLatitude1Longitude1 = ReverseGeocodeFakes.WithCoordinate(Latitude1, Longitude1);

	private static readonly ExifData ExifDataCoordinateLatitude0Longitude0 = ExifDataFakes.WithCoordinateAndReverseGeocode(Latitude0, Longitude0, ReverseGeocodesLatitude0Longitude0);
	private static readonly ExifData ExifDataCoordinateLatitude1Longitude1 = ExifDataFakes.WithCoordinateAndReverseGeocode(Longitude1, Longitude1, ReverseGeocodesLatitude1Longitude1);

	public static TheoryData<List<Photo>, List<PhotoCsv>> CoordinateAndReverseGeocodeData = new()
	{
		{
			[
				PhotoFakes.CreateWithExifData(ExifDataCoordinateLatitude0Longitude0, "photo0.jpg", sourcePath: SourcePath),
				PhotoFakes.CreateWithExifData(ExifDataCoordinateLatitude1Longitude1, "photo1.jpg", sourcePath: SourcePath)
			],
			[
				PhotoCsvFakes.CreateWithFileName(ExifDataCoordinateLatitude0Longitude0, "photo0.jpg", SourcePath, ReverseGeocodesLatitude0Longitude0),
				PhotoCsvFakes.CreateWithFileName(ExifDataCoordinateLatitude1Longitude1, "photo1.jpg", SourcePath, ReverseGeocodesLatitude1Longitude1)
			]
		}
	};

	#endregion

	#region Mixed Data

	private static readonly ExifData ExifDataPhotoTaken = ExifDataFakes.WithPhotoTakenDate(DateTimeFakes.WithYear(2000));

	private const double Latitude = 1;
	private const double Longitude = 1;
	private static readonly List<string> ReverseGeocodes = ReverseGeocodeFakes.WithCoordinate(Latitude, Longitude);
	private static readonly ExifData ExifDataCoordinate = ExifDataFakes.WithCoordinateAndReverseGeocode(1, 1, ReverseGeocodes);

	public static TheoryData<List<Photo>, List<PhotoCsv>> MixedData = new()
	{
		{
			[
				PhotoFakes.CreateWithExifData(ExifData2000, "photo-taken-date.jpg", sourcePath: SourcePath),
				PhotoFakes.CreateWithExifData(ExifDataCoordinate, "coordinate-reverse-geocode.jpg", sourcePath: SourcePath)
			],
			[
				PhotoCsvFakes.CreateWithFileName(ExifDataPhotoTaken, "photo-taken-date.jpg", SourcePath),
				PhotoCsvFakes.CreateWithFileName(ExifDataCoordinateLatitude1Longitude1, "coordinate-reverse-geocode.jpg", SourcePath, ReverseGeocodes)
			]
		}
	};

	#endregion

	[Theory]
	[MemberData(nameof(PhotoDateTakenData))]
	[MemberData(nameof(CoordinateAndReverseGeocodeData))]
	[MemberData(nameof(MixedData))]
	public async Task CsvOutput_Writes_File_And_And_Verify_PhotoCsv_Model_Matched_With_Reading_Output_File(List<Photo> photos, List<PhotoCsv> expectedPhotoCsvModels)
	{
		var outputCsvPath = MockFileSystemHelper.Path("/output.csv");
		var mockFileSystem = new MockFileSystem();
		var sut = new CsvService(mockFileSystem, NullLogger<CsvService>.Instance, ToolOptionFakes.Create(), ConsoleWriterFakes.Valid());
		await sut.CreateInfoReport(photos, outputCsvPath);
		var csvFile = mockFileSystem.FileInfo.New(outputCsvPath);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(csvFile);
		actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
	}

	#endregion

	#region Report

	public static TheoryData<string, List<Photo>, List<PhotoCsv>> ReportWritesToCsvFileAndVerifyPhotoCsvModelMatchedWithReadingOutputFileData = new()
	{
		{
			OutputPath,
			[
				PhotoFakes.Create("photo0.jpg", Date2000, sourcePath: SourcePath, outputFolder: OutputPath, targetRelativeDirectoryPath: "")
			],
			[
				PhotoCsvFakes.WithTargetRelativePath(ExifData2000, "photo0.jpg", SourcePath, targetRelativePath: "photo0.jpg")
			]
		},
	};

	[Theory]
	[MemberData(nameof(ReportWritesToCsvFileAndVerifyPhotoCsvModelMatchedWithReadingOutputFileData))]
	public async Task CopyWithReport_Writes_To_Csv_File_And_Verify_PhotoCsv_Model_Matched_With_Reading_Output_File(string output, IEnumerable<Photo> photos, List<PhotoCsv> expectedPhotoCsvModels)
	{
		var outputFolder = MockFileSystemHelper.Path(output);
		var mockFileSystem = new MockFileSystem();
		var sut = new CsvService(mockFileSystem, NullLogger<CsvService>.Instance, ToolOptionFakes.Create(), ConsoleWriterFakes.Valid());
		await sut.CreateCopyReport(photos, outputFolder);
		var csvFile = mockFileSystem.FileInfo.New(Path.Combine(outputFolder, ToolOptionFakes.CsvReportFileName));
		csvFile.Exists.Should().BeTrue();
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(csvFile);
		actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
	}

	[Theory]
	[MemberData(nameof(ReportWritesToCsvFileAndVerifyPhotoCsvModelMatchedWithReadingOutputFileData))]
	public async Task DryRun_Report_Writes_To_Csv_File_And_Verify_PhotoCsv_Model_Matched_With_Reading_Output_File(string output, IEnumerable<Photo> photos, List<PhotoCsv> expectedPhotoCsvModels)
	{
		var outputFolder = MockFileSystemHelper.Path(output);
		var mockFileSystem = new MockFileSystem();
		var sut = new CsvService(mockFileSystem, NullLogger<CsvService>.Instance, ToolOptionFakes.Create(), ConsoleWriterFakes.Valid());
		await sut.CreateCopyReport(photos, outputFolder);
		var csvFile = mockFileSystem.FileInfo.New(Path.Combine(outputFolder, "photo-cli-report.csv"));
		csvFile.Exists.Should().BeTrue();
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(csvFile);
		actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
	}

	#endregion
}
