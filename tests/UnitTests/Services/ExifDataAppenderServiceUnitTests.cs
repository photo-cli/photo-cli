using Moq.Language.Flow;

namespace PhotoCli.Tests.UnitTests.Services;

public class ExifDataAppenderServiceUnitTests
{
	private readonly Mock<IExifParserService> _parsePhotoTakenDateTime;
	private readonly ISetup<IExifParserService, ExifData?> _setupParseExifData;

	public ExifDataAppenderServiceUnitTests()
	{
		_parsePhotoTakenDateTime = new Mock<IExifParserService>();
		_setupParseExifData = _parsePhotoTakenDateTime.Setup(e => e.Parse(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()));
	}

	public static TheoryData<Dictionary<string, ExifData>> ValidData = new()
	{
		new Dictionary<string, ExifData>
		{
			{ "path2000", ExifDataFakes.WithYear(2000) }
		},
		new Dictionary<string, ExifData>
		{
			{ "path2001", ExifDataFakes.WithYear(2001) },
			{ "path2002", ExifDataFakes.WithYear(2002) }
		}
	};

	public static TheoryData<Dictionary<string, ExifData>> NoPhotoTakenDateData = new()
	{
		new Dictionary<string, ExifData>
		{
			{ "path", ExifDataFakes.WithNoPhotoTakenDate() }
		}
	};


	[Theory]
	[MemberData(nameof(ValidData))]
	[MemberData(nameof(NoPhotoTakenDateData))]
	public void PhotoTakenByPath_Returns_Dictionary_Key_As_Path_And_Value_As_PhotoTaken(Dictionary<string, ExifData> photoExifDataDictionarySample)
	{
		_setupParseExifData.Returns((string path, bool _, bool _) => photoExifDataDictionarySample[path]);
		var sut = new ExifDataAppenderService(_parsePhotoTakenDateTime.Object, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		var photoTakenDictionary = sut.ExifDataByPath(photoExifDataDictionarySample.Keys.ToArray(), out _, out _, out _);
		photoTakenDictionary.Should().BeEquivalentTo(photoExifDataDictionarySample);
	}
}
