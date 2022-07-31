using System.IO.Abstractions;

namespace PhotoCli.Tests.UnitTests;

public class AddressRunnerUnitTests
{
	private readonly Mock<IExifParserService> _photoExifParserServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IReverseGeocodeService> _reverseGeocodeServiceMock = new(MockBehavior.Strict);
	private readonly Mock<IFileSystem> _fileSystemMock = new();

	#region Expected Code Flow

	public static TheoryData<AddressListType> AddressListTypes = new()
	{
		AddressListType.FullResponse,
		AddressListType.SelectedProperties,
		AddressListType.AllAvailableProperties,
	};

	[Theory]
	[MemberData(nameof(AddressListTypes))]
	public async Task Valid_Code_Flow_Verify_All_Invocations_With_Specific_Parameters_Should_Exit_With_Success(AddressListType addressListType)
	{
		var options = AddressOptionsFakes.WithAddressListType(addressListType);
		var coordinate = CoordinateFakes.Valid();
		SetupMockFileSystem(true);
		_photoExifParserServiceMock.Setup(s => s.Parse(It.IsAny<string>(), false, true)).Returns(() => ExifDataFakes.WithCoordinate(coordinate));

		string expectedConsoleOutput;
		switch (addressListType)
		{
			case AddressListType.FullResponse:
				const string validFullResponse = "valid-json";
				_reverseGeocodeServiceMock.Setup(s => s.RawResponse(coordinate)).Returns(() => Task.FromResult(validFullResponse));
				expectedConsoleOutput = validFullResponse;
				break;
			case AddressListType.SelectedProperties:
				_reverseGeocodeServiceMock.Setup(s => s.Get(coordinate)).Returns(() => Task.FromResult(ReverseGeocodeFakes.Valid().AsEnumerable()));
				expectedConsoleOutput = $"Country{Environment.NewLine}City{Environment.NewLine}Neighbourhood";
				break;
			case AddressListType.AllAvailableProperties:
				_reverseGeocodeServiceMock.Setup(s => s.AllAvailableReverseGeocodes(coordinate)).Returns(() => Task.FromResult(new Dictionary<string, object>
				{
					{ "property-test1", "property-value1" }
				}));
				expectedConsoleOutput = "property-test1: property-value1";
				break;
			default:
				throw new Exception($"Not defined: {nameof(addressListType)}");
		}

		var consoleWriterMock = new Mock<IConsoleWriter>();
		var sut = new AddressRunner(_photoExifParserServiceMock.Object, _reverseGeocodeServiceMock.Object, options, consoleWriterMock.Object, _fileSystemMock.Object);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.Success);
		consoleWriterMock.Verify(v => v.Write(expectedConsoleOutput));
		VerifyAll();
	}

	#endregion

	#region Breaking Code Flow

	[Fact]
	public async Task Photo_With_No_Coordinate_Should_Exit_With_PhotosWithNoCoordinatePreventedProcess()
	{
		var options = AddressOptionsFakes.Valid();
		SetupMockFileSystem(true);
		_photoExifParserServiceMock.Setup(s => s.Parse(It.IsAny<string>(), false, true)).Returns(ExifDataFakes.WithNoCoordinate);
		var sut = new AddressRunner(_photoExifParserServiceMock.Object, _reverseGeocodeServiceMock.Object, options, ConsoleWriterFakes.Valid(), _fileSystemMock.Object);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.PhotosWithNoCoordinatePreventedProcess);
		VerifyAll();
	}

	[Fact]
	public async Task Not_Exists_File_Path_Should_Exit_With_InputFileNotExists()
	{
		var options = AddressOptionsFakes.Valid();
		SetupMockFileSystem(false);
		var sut = new AddressRunner(_photoExifParserServiceMock.Object, _reverseGeocodeServiceMock.Object, options, ConsoleWriterFakes.Valid(), _fileSystemMock.Object);
		var exitCode = await sut.Execute();
		exitCode.Should().Be(ExitCode.InputFileNotExists);
	}

	#endregion

	#region Utils

	private void SetupMockFileSystem(bool fileExists)
	{
		_fileSystemMock.Setup(s => s.FileInfo).Returns(() => new MockFileInfoFactory(new MockFileSystem()));
		_fileSystemMock.SetupGet(s => s.FileInfo.FromFileName(It.IsAny<string>()).Exists).Returns(() => fileExists);
	}

	private void VerifyAll()
	{
		_photoExifParserServiceMock.VerifyAll();
		_reverseGeocodeServiceMock.VerifyAll();

		_photoExifParserServiceMock.VerifyNoOtherCalls();
		_reverseGeocodeServiceMock.VerifyNoOtherCalls();
	}

	#endregion
}
