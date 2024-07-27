namespace PhotoCli.Tests.UnitTests.Services;

public class ExifDataAppenderServiceUnitTests
{
	#region Setting Exif Data

	public static TheoryData<Dictionary<Photo, ExifData>, IReadOnlyList<Photo>> ValidExifData = new()
	{
		{
			new Dictionary<Photo, ExifData>
			{
				{
					PhotoFakes.WithSourcePathAndExifData("valid-exif.jpg", ExifDataFakes.Valid()),
					ExifDataFakes.Valid()
				},
			},
			[
				PhotoFakes.WithSourcePathAndExifData("valid-exif.jpg", ExifDataFakes.Valid()),
			]
		},
		{
			new Dictionary<Photo, ExifData>
			{
				{
					PhotoFakes.WithSourcePathAndExifData("photo-taken-date.jpg", ExifDataFakes.WithYear(2000)),
					ExifDataFakes.WithYear(2000)
				},
				{
					PhotoFakes.WithSourcePathAndExifData("coordinate.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
					ExifDataFakes.WithCoordinateSampleId(1)
				},
			},
			[
				PhotoFakes.WithSourcePathAndExifData("photo-taken-date.jpg", ExifDataFakes.WithYear(2000)),
				PhotoFakes.WithSourcePathAndExifData("coordinate.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
			]
		},
	};

	public static TheoryData<Dictionary<Photo, ExifData?>, IReadOnlyList<Photo>> InvalidExifData = new()
	{
		{
			new Dictionary<Photo, ExifData?>
			{
				{
					PhotoFakes.WithSourcePathAndExifData("invalid-exif.jpg", null),
					null
				},
			},
			[
				PhotoFakes.WithSourcePathAndExifData("invalid-exif.jpg", null),
			]
		},
		{
			new Dictionary<Photo, ExifData?>
			{
				{
					PhotoFakes.WithSourcePathAndExifData("invalid-1.jpg", null),
					null
				},
				{
					PhotoFakes.WithSourcePathAndExifData("invalid-2.jpg", null),
					null
				},
			},
			[
				PhotoFakes.WithSourcePathAndExifData("invalid-1.jpg", null),
				PhotoFakes.WithSourcePathAndExifData("invalid-2.jpg", null),
			]
		},
	};

	[Theory]
	[MemberData(nameof(ValidExifData))]
	[MemberData(nameof(InvalidExifData))]
	public void Given_Photos_Should_Match_With_Photos_With_Exif(Dictionary<Photo, ExifData> exifDataByPhoto, IReadOnlyList<Photo> expectedOutputPhotos)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var actualPhotos = sut.ExtractExifData(inputPhotos, out _, out _, out _);
		actualPhotos.Should().BeEquivalentTo(expectedOutputPhotos);
	}

	#endregion

	#region AllPhotosAreValid Parameter

	public static TheoryData<Dictionary<Photo, ExifData>> AllValidPhotos = new()
	{
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("valid-exif.jpg", ExifDataFakes.Valid()),
				ExifDataFakes.Valid()
			},
		},
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("photo-taken-date.jpg", ExifDataFakes.WithYear(2000)),
				ExifDataFakes.WithYear(2000)
			},
			{
				PhotoFakes.WithSourcePathAndExifData("coordinate.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
				ExifDataFakes.WithCoordinateSampleId(1)
			},
		},
	};

	[Theory]
	[MemberData(nameof(AllValidPhotos))]
	public void Given_Valid_Photos_Should_Return_AllPhotosAreValid_As_True(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		sut.ExtractExifData(inputPhotos, out var allPhotosAreValid, out _, out _);
		allPhotosAreValid.Should().BeTrue();
	}

	public static TheoryData<Dictionary<Photo, ExifData?>> ContainsInvalidPhoto = new()
	{
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("invalid.jpg", ExifDataFakes.WithInvalidFileFormat()),
				ExifDataFakes.WithInvalidFileFormat()
			},
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("valid.jpg", ExifDataFakes.Valid()),
				ExifDataFakes.Valid()
			},
			{
				PhotoFakes.WithSourcePathAndExifData("invalid.jpg", ExifDataFakes.WithInvalidFileFormat()),
				ExifDataFakes.WithInvalidFileFormat()
			},
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("valid-1.jpg", ExifDataFakes.ValidSampleId(1)),
				ExifDataFakes.Valid()
			},
			{
				PhotoFakes.WithSourcePathAndExifData("invalid.jpg", ExifDataFakes.WithInvalidFileFormat()),
				ExifDataFakes.WithInvalidFileFormat()
			},
			{
				PhotoFakes.WithSourcePathAndExifData("valid-2.jpg", ExifDataFakes.ValidSampleId(2)),
				ExifDataFakes.Valid()
			},
		},
	};

	[Theory]
	[MemberData(nameof(ContainsInvalidPhoto))]
	public void Given_Photos_That_Contain_Invalid_Should_Return_AllPhotosAreValid_As_False(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		sut.ExtractExifData(inputPhotos, out var allPhotosAreValid, out _, out _);
		allPhotosAreValid.Should().BeFalse();
	}

	#endregion

	#region AllPhotosHasPhotoTaken Parameter

	public static TheoryData<Dictionary<Photo, ExifData>> AllPhotoTakenDatePhotos = new()
	{
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-photo-taken.jpg", ExifDataFakes.PhotoTakenDateSampleId(1)),
				ExifDataFakes.PhotoTakenDateSampleId(1)
			},
		},
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-photo-taken-1.jpg", ExifDataFakes.PhotoTakenDateSampleId(1)),
				ExifDataFakes.PhotoTakenDateSampleId(1)
			},
			{
				PhotoFakes.WithSourcePathAndExifData("has-photo-taken-2.jpg", ExifDataFakes.PhotoTakenDateSampleId(2)),
				ExifDataFakes.PhotoTakenDateSampleId(2)
			},
		},
	};

	[Theory]
	[MemberData(nameof(AllPhotoTakenDatePhotos))]
	public void Given_All_Photo_Has_Taken_Date_Should_Return_AllPhotosHasPhotoTaken_As_True(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		sut.ExtractExifData(inputPhotos, out _, out var allPhotosHasPhotoTaken, out _);
		allPhotosHasPhotoTaken.Should().BeTrue();
	}

	public static TheoryData<Dictionary<Photo, ExifData?>> ContainsNoPhotoTakenDatePhoto = new()
	{
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("no-photo-taken.jpg", ExifDataFakes.WithNoPhotoTakenDate()),
				ExifDataFakes.WithNoPhotoTakenDate()
			},
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-photo-taken.jpg", ExifDataFakes.PhotoTakenDateSampleId(1)),
				ExifDataFakes.PhotoTakenDateSampleId(1)
			},
			{
				PhotoFakes.WithSourcePathAndExifData("no-photo-taken.jpg", ExifDataFakes.WithNoPhotoTakenDate()),
				ExifDataFakes.WithNoPhotoTakenDate()
			},
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-photo-taken-1.jpg", ExifDataFakes.PhotoTakenDateSampleId(1)),
				ExifDataFakes.PhotoTakenDateSampleId(1)
			},
			{
				PhotoFakes.WithSourcePathAndExifData("no-photo-taken.jpg", ExifDataFakes.WithNoPhotoTakenDate()),
				ExifDataFakes.WithNoPhotoTakenDate()
			},
			{
				PhotoFakes.WithSourcePathAndExifData("has-photo-taken-2.jpg", ExifDataFakes.PhotoTakenDateSampleId(2)),
				ExifDataFakes.PhotoTakenDateSampleId(2)
			},
		},
	};

	[Theory]
	[MemberData(nameof(ContainsNoPhotoTakenDatePhoto))]
	public void Given_Photos_That_Contain_No_Photo_Taken_Date_Should_Return_AllPhotosAreValid_As_False(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		sut.ExtractExifData(inputPhotos, out _, out var allPhotosHasPhotoTaken, out _);
		allPhotosHasPhotoTaken.Should().BeFalse();
	}

	#endregion

	#region AllPhotosHasCoordinate Parameter

	public static TheoryData<Dictionary<Photo, ExifData>> AllPhotoHasCoordinatePhotos = new()
	{
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-coordinate.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
				ExifDataFakes.WithCoordinateSampleId(1)
			},
		},
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-coordinate-1.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
				ExifDataFakes.WithCoordinateSampleId(1)
			},
			{
				PhotoFakes.WithSourcePathAndExifData("has-coordinate-2.jpg", ExifDataFakes.WithCoordinateSampleId(2)),
				ExifDataFakes.WithCoordinateSampleId(2)
			},
		},
	};

	[Theory]
	[MemberData(nameof(AllPhotoHasCoordinatePhotos))]
	public void Given_All_Photo_Has_Coordinate_Should_Return_AllPhotosHasCoordinate_As_True(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		sut.ExtractExifData(inputPhotos, out _, out _, out var allPhotosHasCoordinate);
		allPhotosHasCoordinate.Should().BeTrue();
	}

	public static TheoryData<Dictionary<Photo, ExifData?>> ContainsNoPhotoCoordinatePhoto = new()
	{
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("no-coordinate.jpg", ExifDataFakes.WithNoCoordinate()),
				ExifDataFakes.WithNoCoordinate()
			},
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-coordinate.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
				ExifDataFakes.WithCoordinateSampleId(1)
			},
			{
				PhotoFakes.WithSourcePathAndExifData("no-coordinate.jpg", ExifDataFakes.WithNoCoordinate()),
				ExifDataFakes.WithNoCoordinate()
			},
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoFakes.WithSourcePathAndExifData("has-coordinate-1.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
				ExifDataFakes.WithCoordinateSampleId(1)
			},
			{
				PhotoFakes.WithSourcePathAndExifData("no-coordinate.jpg", ExifDataFakes.WithNoCoordinate()),
				ExifDataFakes.WithNoCoordinate()
			},
			{
				PhotoFakes.WithSourcePathAndExifData("has-coordinate-2.jpg", ExifDataFakes.WithCoordinateSampleId(2)),
				ExifDataFakes.WithCoordinateSampleId(2)
			},
		},
	};

	[Theory]
	[MemberData(nameof(ContainsNoPhotoCoordinatePhoto))]
	public void Given_Photos_That_Contain_No_Photo_Coordinate_Should_Return_AllPhotosHasPhotoTaken_As_False(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		sut.ExtractExifData(inputPhotos, out _, out _, out var allPhotosHasCoordinate);
		allPhotosHasCoordinate.Should().BeFalse();
	}

	#endregion

	private static (ExifDataAppenderService, List<Photo>) SetupExifDataByPhoto(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var inputPhotos = new List<Photo>();
		var exifParseServiceMock = new Mock<IExifParserService>(MockBehavior.Strict);
		foreach (var (photo, exifData) in exifDataByPhoto)
		{
			exifParseServiceMock.Setup(e => e.Parse(photo.PhotoFile.SourcePath, It.IsAny<bool>(), It.IsAny<bool>())).Returns(exifData);
			inputPhotos.Add(photo);
		}

		var sut = new ExifDataAppenderService(exifParseServiceMock.Object, StatisticsFakes.Empty(), ConsoleWriterFakes.Valid());
		return (sut, inputPhotos);
	}
}
