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
					PhotoInputWithFileName("valid-exif.jpg"), ExifDataFakes.Valid()
				}
			},
			[
				PhotoOutputWithFileNameToVerify("valid-exif.jpg", ExifDataFakes.Valid()),
			]
		},
		{
			new Dictionary<Photo, ExifData>
			{
				{
					PhotoInputWithFileName("photo-taken-date.jpg"), ExifDataFakes.WithYear(2000)
				},
				{
					PhotoInputWithFileName("coordinate.jpg"), ExifDataFakes.WithCoordinateSampleId(1)
				}
			},
			[
				PhotoOutputWithFileNameToVerify("photo-taken-date.jpg", ExifDataFakes.WithYear(2000)),
				PhotoOutputWithFileNameToVerify("coordinate.jpg", ExifDataFakes.WithCoordinateSampleId(1)),
			]
		},
	};

	public static TheoryData<Dictionary<Photo, ExifData?>, IReadOnlyList<Photo>> InvalidExifData = new()
	{
		{
			new Dictionary<Photo, ExifData?>
			{
				{
					PhotoInputWithFileName("invalid-exif.jpg"), null
				}
			},
			[
				PhotoOutputWithFileNameToVerify("invalid-exif.jpg", null),
			]
		},
		{
			new Dictionary<Photo, ExifData?>
			{
				{
					PhotoInputWithFileName("invalid-1.jpg"), null
				},
				{
					PhotoInputWithFileName("invalid-2.jpg"), null
				}
			},
			[
				PhotoOutputWithFileNameToVerify("invalid-1.jpg", null),
				PhotoOutputWithFileNameToVerify("invalid-2.jpg", null),
			]
		},
	};

	[Theory]
	[MemberData(nameof(ValidExifData))]
	[MemberData(nameof(InvalidExifData))]
	public void Given_Photos_Should_Match_With_Photos_With_Exif(Dictionary<Photo, ExifData> exifDataByPhoto, IReadOnlyList<Photo> expectedOutputPhotos)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.Photos.Should().BeEquivalentTo(expectedOutputPhotos);
	}

	#endregion

	#region AllPhotosAreValid Parameter

	public static TheoryData<Dictionary<Photo, ExifData>> AllValidPhotos = new()
	{
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoInputDummy(), ExifDataFakes.Valid()
			}
		},
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoInputDummy(), ExifDataFakes.WithYear(2000)
			},
			{
				PhotoInputDummy(), ExifDataFakes.WithCoordinateSampleId(1)
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllValidPhotos))]
	public void Given_Valid_Photos_Should_Return_AllPhotosAreValid_As_True(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.AllPhotosAreValid.Should().BeTrue();
	}

	public static TheoryData<Dictionary<Photo, ExifData?>> ContainsInvalidPhoto = new()
	{
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputDummy(), ExifDataFakes.WithInvalidFileFormat()
			}
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputDummy(), ExifDataFakes.Valid()
			},
			{
				PhotoInputDummy(), ExifDataFakes.WithInvalidFileFormat()
			}
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputDummy(), ExifDataFakes.Valid()
			},
			{
				PhotoInputDummy(), ExifDataFakes.WithInvalidFileFormat()
			},
			{
				PhotoInputDummy(), ExifDataFakes.Valid()
			},
		},
	};

	[Theory]
	[MemberData(nameof(ContainsInvalidPhoto))]
	public void Given_Photos_That_Contain_Invalid_Should_Return_AllPhotosAreValid_As_False(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.AllPhotosAreValid.Should().BeFalse();
	}

	#endregion

	#region AllPhotosHasPhotoTaken Parameter

	public static TheoryData<Dictionary<Photo, ExifData>> AllPhotoTakenDatePhotos = new()
	{
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoInputDummy(), ExifDataFakes.PhotoTakenDateSampleId(1)
			}
		},
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoInputDummy(), ExifDataFakes.PhotoTakenDateSampleId(1)
			},
			{
				PhotoInputDummy(), ExifDataFakes.PhotoTakenDateSampleId(2)
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllPhotoTakenDatePhotos))]
	public void Given_All_Photo_Has_Taken_Date_Should_Return_AllPhotosHasPhotoTaken_As_True(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.AllPhotosHasPhotoTaken.Should().BeTrue();
	}

	public static TheoryData<Dictionary<Photo, ExifData?>> ContainsNoPhotoTakenDatePhoto = new()
	{
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputWithFileName("no-photo-taken.jpg"), ExifDataFakes.WithNoPhotoTakenDate()
			}
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputWithFileName("has-photo-taken.jpg"), ExifDataFakes.PhotoTakenDateSampleId(1)
			},
			{
				PhotoInputWithFileName("no-photo-taken.jpg"), ExifDataFakes.WithNoPhotoTakenDate()
			}
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputWithFileName("has-photo-taken-1.jpg"), ExifDataFakes.PhotoTakenDateSampleId(1)
			},
			{
				PhotoInputWithFileName("no-photo-taken.jpg"), ExifDataFakes.WithNoPhotoTakenDate()
			},
			{
				PhotoInputWithFileName("has-photo-taken-2.jpg"), ExifDataFakes.PhotoTakenDateSampleId(2)
			}
		},
	};

	[Theory]
	[MemberData(nameof(ContainsNoPhotoTakenDatePhoto))]
	public void Given_Photos_That_Contain_No_Photo_Taken_Date_Should_Return_AllPhotosAreValid_As_False(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.AllPhotosHasPhotoTaken.Should().BeFalse();
	}

	#endregion

	#region AllPhotosHasCoordinate Parameter

	public static TheoryData<Dictionary<Photo, ExifData>> AllPhotoHasCoordinatePhotos = new()
	{
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoInputWithFileName("has-coordinate.jpg"), ExifDataFakes.WithCoordinateSampleId(1)
			}
		},
		new Dictionary<Photo, ExifData>
		{
			{
				PhotoInputWithFileName("has-coordinate-1.jpg"), ExifDataFakes.WithCoordinateSampleId(1)
			},
			{
				PhotoInputWithFileName("has-coordinate-2.jpg"), ExifDataFakes.WithCoordinateSampleId(2)
			}
		},
	};

	[Theory]
	[MemberData(nameof(AllPhotoHasCoordinatePhotos))]
	public void Given_All_Photo_Has_Coordinate_Should_Return_AllPhotosHasCoordinate_As_True(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.AllPhotosHasCoordinate.Should().BeTrue();
	}

	public static TheoryData<Dictionary<Photo, ExifData?>> ContainsNoPhotoCoordinatePhoto = new()
	{
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputWithFileName("no-coordinate.jpg"), ExifDataFakes.WithNoCoordinate()
			}
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputWithFileName("has-coordinate.jpg"), ExifDataFakes.WithCoordinateSampleId(1)
			},
			{
				PhotoInputWithFileName("no-coordinate.jpg"), ExifDataFakes.WithNoCoordinate()
			}
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputWithFileName("has-coordinate-1.jpg"), ExifDataFakes.WithCoordinateSampleId(1)
			},
			{
				PhotoInputWithFileName("no-coordinate.jpg"), ExifDataFakes.WithNoCoordinate()
			},
			{
				PhotoInputWithFileName("has-coordinate-2.jpg"), ExifDataFakes.WithCoordinateSampleId(2)
			}
		},
	};

	[Theory]
	[MemberData(nameof(ContainsNoPhotoCoordinatePhoto))]
	public void Given_Photos_That_Contain_No_Photo_Coordinate_Should_Return_AllPhotosHasPhotoTaken_As_False(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.AllPhotosHasCoordinate.Should().BeFalse();
	}

	#endregion

	#region AlbumDateRanges

	public static TheoryData<Dictionary<Photo, ExifData>, AlbumDateRange?> PhotosTakenDateCalculatedRangeMatchingWithExpectedAlbumRange = new()
	{
		{
			new Dictionary<Photo, ExifData>
			{
				{
					PhotoInputDummy(), ExifDataFakes.WithYear(2000)
				},
				{
					PhotoInputDummy(), ExifDataFakes.WithYear(2001)
				}
			},
			AlbumDateRangeFakes.WithStartEnd(DateTimeFakes.WithYear(2000), DateTimeFakes.WithYear(2001))
		},
		{
			new Dictionary<Photo, ExifData>
			{
				{
					PhotoInputDummy(), ExifDataFakes.WithPhotoTakenDate(new DateTime(2007, 3, 14, 18, 45, 42))
				},
				{
					PhotoInputDummy(), ExifDataFakes.WithPhotoTakenDate(new DateTime(2004, 11, 29, 9, 12, 37))
				}
			},
			AlbumDateRangeFakes.WithStartEnd(new DateTime(2004, 11, 29, 9, 12, 37), new DateTime(2007, 3, 14, 18, 45, 42))
		},
		{
			new Dictionary<Photo, ExifData>
			{
				{
					PhotoInputDummy(), ExifDataFakes.WithPhotoTakenDate(new DateTime(2016, 7, 21, 23, 2, 9))
				},
				{
					PhotoInputDummy(), ExifDataFakes.WithPhotoTakenDate(new DateTime(2037, 3, 1, 13, 59, 34))
				},
				{
					PhotoInputDummy(), ExifDataFakes.WithPhotoTakenDate(new DateTime(2007, 12, 3, 2, 19, 0))
				},
				{
					PhotoInputDummy(), ExifDataFakes.WithPhotoTakenDate(new DateTime(2012, 10, 23, 16, 12, 45))
				}
			},
			AlbumDateRangeFakes.WithStartEnd(new DateTime(2007, 12, 3, 2, 19, 0), new DateTime(2037, 3, 1, 13, 59, 34))
		},
	};

	[Theory]
	[MemberData(nameof(PhotosTakenDateCalculatedRangeMatchingWithExpectedAlbumRange))]
	public void ExtractExifData_GivenPhotosWithDateTaken_ShouldMatchWithTheDateRange(Dictionary<Photo, ExifData> exifDataByPhoto, AlbumDateRange? expectedAlbumDateRange)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.DateRange.Should().BeEquivalentTo(expectedAlbumDateRange);
	}

	public static TheoryData<Dictionary<Photo, ExifData?>> PhotosWithNoPhotoDateTaken = new()
	{
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputDummy(), ExifDataFakes.WithNoPhotoTakenDate()
			}
		},
		new Dictionary<Photo, ExifData?>
		{
			{
				PhotoInputDummy(), ExifDataFakes.WithNoPhotoTakenDate()
			},
			{
				PhotoInputDummy(), null
			}
		},
		new Dictionary<Photo, ExifData?>(),
	};

	[Theory]
	[MemberData(nameof(PhotosWithNoPhotoDateTaken))]
	public void ExtractExifData_GivenPhotosWithNoPhotoDateTaken_ShouldReturnDateRangeAsNull(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var (sut, inputPhotos) = SetupExifDataByPhoto(exifDataByPhoto);
		var exifDataResult = sut.ExtractExifData(inputPhotos);
		exifDataResult.DateRange.Should().BeNull();
	}

	#endregion

	private static Photo PhotoInputWithFileName(string fileNameWithExtension)
	{
		return PhotoFakes.WithSourcePathAndWithoutExifData(fileNameWithExtension);
	}

	private static Photo PhotoInputDummy()
	{
		return PhotoFakes.WithSourcePathAndWithoutExifData($"{Guid.NewGuid()}.jpg");
	}

	private static Photo PhotoOutputWithFileNameToVerify(string fileNameWithExtension, ExifData? exifData)
	{
		return PhotoFakes.WithSourcePathAndExifData(fileNameWithExtension, exifData);
	}

	private static (ExifDataAppenderService, List<Photo>) SetupExifDataByPhoto(Dictionary<Photo, ExifData> exifDataByPhoto)
	{
		var inputPhotos = new List<Photo>();
		var exifParseServiceMock = new Mock<IExifParserService>(MockBehavior.Strict);
		foreach (var (photo, exifData) in exifDataByPhoto)
		{
			exifParseServiceMock.Setup(e => e
				.Parse(photo.PhotoFile.SourcePath, It.IsAny<bool>(), It.IsAny<bool>()))
				.Returns(exifData);

			inputPhotos.Add(photo);
		}

		var sut = new ExifDataAppenderService(exifParseServiceMock.Object);
		return (sut, inputPhotos);
	}
}
