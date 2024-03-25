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

	public static TheoryData<Dictionary<string, ExifData>, List<PhotoCsv>> PhotoDateTakenData = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ MockFileSystemHelper.Path(SourcePath, "photo0.jpg"), ExifData2000 },
				{ MockFileSystemHelper.Path(SourcePath, "photo1.jpg"), ExifData2001 },
			},
			new List<PhotoCsv>
			{
				PhotoCsvFakes.CreateWithFileName("photo0.jpg", SourcePath, Date2000),
				PhotoCsvFakes.CreateWithFileName("photo1.jpg", SourcePath, Date2001),
			}
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

	public static TheoryData<Dictionary<string, ExifData>, List<PhotoCsv>> CoordinateAndReverseGeocodeData = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ MockFileSystemHelper.Path(SourcePath, "photo0.jpg"), ExifDataFakes.WithCoordinateAndReverseGeocode(0, 0, ReverseGeocodesLatitude0Longitude0) },
				{ MockFileSystemHelper.Path(SourcePath, "photo1.jpg"), ExifDataFakes.WithCoordinateAndReverseGeocode(1, 1, ReverseGeocodesLatitude1Longitude1) }
			},
			new List<PhotoCsv>
			{
				PhotoCsvFakes.CreateWithFileName("photo0.jpg", SourcePath, reverseGeocodes: ReverseGeocodesLatitude0Longitude0, latitude: Latitude0, longitude: Longitude0),
				PhotoCsvFakes.CreateWithFileName("photo1.jpg", SourcePath, reverseGeocodes: ReverseGeocodesLatitude1Longitude1, latitude: Latitude1, longitude: Longitude1),
			}
		}
	};

	#endregion

	#region Mixed Data

	public static TheoryData<Dictionary<string, ExifData>, List<PhotoCsv>> MixedData = new()
	{
		{
			new Dictionary<string, ExifData>
			{
				{ MockFileSystemHelper.Path(SourcePath, "photo-photo-taken-date.jpg"), ExifData2000 },
				{ MockFileSystemHelper.Path(SourcePath, "photo-coordinate-reverse-geocode.jpg"), ExifDataFakes.WithCoordinateAndReverseGeocode(0, 0, ReverseGeocodesLatitude0Longitude0) },
			},
			new List<PhotoCsv>
			{
				PhotoCsvFakes.CreateWithFileName("photo-photo-taken-date.jpg", SourcePath, Date2000),
				PhotoCsvFakes.CreateWithFileName("photo-coordinate-reverse-geocode.jpg", SourcePath, reverseGeocodes: ReverseGeocodesLatitude0Longitude0, latitude: Latitude0, longitude: Longitude0),
			}
		}
	};

	#endregion

	[Theory]
	[MemberData(nameof(PhotoDateTakenData))]
	[MemberData(nameof(CoordinateAndReverseGeocodeData))]
	[MemberData(nameof(MixedData))]
	public async Task CsvOutput_Writes_File_And_And_Verify_PhotoCsv_Model_Matched_With_Reading_Output_File(Dictionary<string, ExifData> exifData,
		List<PhotoCsv> expectedPhotoCsvModels)
	{
		var outputCsvPath = MockFileSystemHelper.Path("/output.csv");
		var mockFileSystem = new MockFileSystem();
		var sut = new CsvService(mockFileSystem, NullLogger<CsvService>.Instance, ToolOptionFakes.Create(), ConsoleWriterFakes.Valid());
		await sut.WriteExifDataToCsvOutput(exifData!, outputCsvPath);
		var csvFile = mockFileSystem.FileInfo.New(outputCsvPath);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(csvFile);
		actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
	}

	#endregion

	#region Report

	public static TheoryData<string, IEnumerable<Photo>, List<PhotoCsv>> ReportWritesToCsvFileAndVerifyPhotoCsvModelMatchedWithReadingOutputFileData =
		new()
		{
			{
				OutputPath,
				new List<Photo> { PhotoFakes.Create("photo0.jpg", Date2000, sourcePath: SourcePath) },
				new List<PhotoCsv>
				{
					PhotoCsvFakes.CreateWithFileName("photo0.jpg", SourcePath, Date2000, outputPath: OutputPath),
				}
			},
		};

	[Theory]
	[MemberData(nameof(ReportWritesToCsvFileAndVerifyPhotoCsvModelMatchedWithReadingOutputFileData))]
	public async Task CopyWithReport_Report_Writes_To_Csv_File_And_Verify_PhotoCsv_Model_Matched_With_Reading_Output_File(string output,
		IEnumerable<Photo> photos, List<PhotoCsv> expectedPhotoCsvModels)
	{
		var outputFolder = MockFileSystemHelper.Path(output);
		var mockFileSystem = new MockFileSystem();
		var sut = new CsvService(mockFileSystem, NullLogger<CsvService>.Instance, ToolOptionFakes.Create(), ConsoleWriterFakes.Valid());
		await sut.Report(photos, outputFolder);
		var csvFile = mockFileSystem.FileInfo.New(Path.Combine(outputFolder, ToolOptionFakes.CsvReportFileName));
		csvFile.Exists.Should().BeTrue();
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(csvFile);
		actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
	}

	[Theory]
	[MemberData(nameof(ReportWritesToCsvFileAndVerifyPhotoCsvModelMatchedWithReadingOutputFileData))]
	public async Task DryRun_Report_Writes_To_Csv_File_And_Verify_PhotoCsv_Model_Matched_With_Reading_Output_File(string output,
		IEnumerable<Photo> photos, List<PhotoCsv> expectedPhotoCsvModels)
	{
		var outputFolder = MockFileSystemHelper.Path(output);
		var mockFileSystem = new MockFileSystem();
		var sut = new CsvService(mockFileSystem, NullLogger<CsvService>.Instance, ToolOptionFakes.Create(), ConsoleWriterFakes.Valid());
		await sut.Report(photos, outputFolder);
		var csvFile = mockFileSystem.FileInfo.New(Path.Combine(outputFolder, "photo-cli-report.csv"));
		csvFile.Exists.Should().BeTrue();
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(csvFile);
		actualPhotoCsvModels.Should().BeEquivalentTo(expectedPhotoCsvModels);
	}

	#endregion
}
