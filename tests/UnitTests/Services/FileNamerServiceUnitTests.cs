namespace PhotoCli.Tests.UnitTests.Services;

public class FileNamerServiceUnitTests
{
	private readonly FileNamerService _sut;
	private readonly Mock<ISequentialNumberEnumeratorService> _sequentialNumberEnumerator;
	private readonly Mock<IExifDataGrouperService> _photoTakenDateGrouper;

	public FileNamerServiceUnitTests()
	{
		_sequentialNumberEnumerator = new Mock<ISequentialNumberEnumeratorService>();
		_photoTakenDateGrouper = new Mock<IExifDataGrouperService>(MockBehavior.Strict);
		_sut = new FileNamerService(_sequentialNumberEnumerator.Object, _photoTakenDateGrouper.Object, ToolOptionFakes.Create(), NullLogger<FileNamerService>.Instance);
	}

	#region Numeric

	public static TheoryData<List<string>> NumericNamingStyleAssignNewNameFromISequentialNumberEnumeratorShouldBeEqualData = new()
	{
		new List<string> { "1" },
		new List<string> { "1", "2" },
		new List<string> { "1", "2", "3" },
		new List<string>(),
	};

	[Theory]
	[MemberData(nameof(NumericNamingStyleAssignNewNameFromISequentialNumberEnumeratorShouldBeEqualData))]
	public void Numeric_NamingStyle_Assign_New_Name_From_ISequentialNumberEnumerator_Should_Be_Equal(List<string> fileNames)
	{
		var orderedPhotoInfos = PhotoFakes.DummyOrderedListWithCount(fileNames.Count);

		_sequentialNumberEnumerator.Setup(s => s.NumberIterator(orderedPhotoInfos.Count, It.IsAny<NumberNamingTextStyle>()))
			.Returns(() => fileNames);

		_sut.SetFileName(
			orderedPhotoInfos,
			NamingStyle.Numeric,
			It.IsAny<NumberNamingTextStyle>()
		);

		_sequentialNumberEnumerator.Verify(v => v.NumberIterator(orderedPhotoInfos.Count, It.IsAny<NumberNamingTextStyle>()), Times.Once);
		VerifyNotOtherCalls();

		var photoFileNewNames = orderedPhotoInfos.Select(s => s.NewName).ToList();
		photoFileNewNames.Should().Equal(fileNames);
	}

	#endregion Numeric

	#region Day

	private static readonly Photo PhotoInfoDay1Hour0 = PhotoFakes.WithDayHour(1, 0);
	private static readonly Photo PhotoInfoDay1Hour1 = PhotoFakes.WithDayHour(1, 1);
	private static readonly Photo PhotoInfoDay2Hour0 = PhotoFakes.WithDayHour(2, 0);
	private static readonly Photo PhotoInfoDay3Hour10 = PhotoFakes.WithDayHour(3, 10);
	private static readonly Photo PhotoInfoDay3Hour14 = PhotoFakes.WithDayHour(3, 14);
	private static readonly Photo PhotoInfoDay3Hour23 = PhotoFakes.WithDayHour(3, 23);

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> EveryDayHasOnePhoto = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{ DateTimeFakes.FormatDay(1), new List<Photo> { PhotoInfoDay1Hour0 } },
				{ DateTimeFakes.FormatDay(2), new List<Photo> { PhotoInfoDay2Hour0 } }
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameDay(1, 0, string.Empty),
				PhotoFakes.WithNewFileNameDay(2, 0, string.Empty)
			}
		}
	};

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> OneDayHasTwoPhotoDayTwoHasOnePhoto = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatDay(1),
					new List<Photo> { PhotoInfoDay1Hour0, PhotoInfoDay1Hour1 }
				},
				{
					DateTimeFakes.FormatDay(2),
					new List<Photo> { PhotoInfoDay2Hour0 }
				}
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameDay(1, 0, "-1"),
				PhotoFakes.WithNewFileNameDay(1, 1, "-2"),
				PhotoFakes.WithNewFileNameDay(2, 0, string.Empty)
			}
		}
	};

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> ThreeDayComplex = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatDay(3),
					new List<Photo> { PhotoInfoDay3Hour10, PhotoInfoDay3Hour14, PhotoInfoDay3Hour23 }
				},
				{
					DateTimeFakes.FormatDay(1),
					new List<Photo> { PhotoInfoDay1Hour0, PhotoInfoDay1Hour1 }
				},
				{
					DateTimeFakes.FormatDay(2),
					new List<Photo> { PhotoInfoDay2Hour0 }
				}
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameDay(3, 10, "-1"),
				PhotoFakes.WithNewFileNameDay(1, 1, "-2"),
				PhotoFakes.WithNewFileNameDay(3, 14, "-2"),
				PhotoFakes.WithNewFileNameDay(2, 0, string.Empty),
				PhotoFakes.WithNewFileNameDay(3, 23, "-3"),
				PhotoFakes.WithNewFileNameDay(1, 0, "-1"),
			}
		}
	};

	[Theory]
	[MemberData(nameof(EveryDayHasOnePhoto))] // Every day there is one photo, so name will be just yyyy-MM-dd. There is no need for number postfix.
	[MemberData(nameof(OneDayHasTwoPhotoDayTwoHasOnePhoto))] // Day one has two photo, it will have number postfix after date like yyyy-MM-dd-{number-postfix}. Day two has one photo there will be no postfix.
	[MemberData(nameof(ThreeDayComplex))] // Three day complex scenario
	[MemberData(nameof(DateNamingStyleEmpty))] // Empty input of PhotoInfos should not break.
	public void Day_NamingStyle_Assign_New_Name_By_Grouping_On_Day(Dictionary<string, List<Photo>> mockGroupedPhotoInfosByDate, List<Photo> expectedPhotoInfosWithNewFileNames)
	{
		VerifyFileNamesOnGroupedByDateDictionaryWithExpected(NamingStyle.Day, mockGroupedPhotoInfosByDate, expectedPhotoInfosWithNewFileNames);
	}

	#endregion Day

	#region Date Time With Minute

	private static readonly Photo PhotoInfoMinute1Second0 = PhotoFakes.WithMinuteSecond(1, 0);
	private static readonly Photo PhotoInfoMinute1Second1 = PhotoFakes.WithMinuteSecond(1, 1);
	private static readonly Photo PhotoInfoMinute2Second0 = PhotoFakes.WithMinuteSecond(2, 0);
	private static readonly Photo PhotoInfoMinute3Second17 = PhotoFakes.WithMinuteSecond(3, 17);
	private static readonly Photo PhotoInfoMinute3Second36 = PhotoFakes.WithMinuteSecond(3, 36);
	private static readonly Photo PhotoInfoMinute3Second54 = PhotoFakes.WithMinuteSecond(3, 54);

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> EveryMinuteHasOnePhoto = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{ DateTimeFakes.FormatMinute(1), new List<Photo> { PhotoInfoMinute1Second0 } },
				{ DateTimeFakes.FormatMinute(2), new List<Photo> { PhotoInfoMinute2Second0 } },
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameMinute(1, 0, string.Empty),
				PhotoFakes.WithNewFileNameMinute(2, 0, string.Empty),
			}
		}
	};

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> MinuteOneHasTwoPhotoDayMinuteTwoHasOnePhoto = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatMinute(1),
					new List<Photo> { PhotoInfoMinute1Second0, PhotoInfoMinute1Second1 }
				},
				{
					DateTimeFakes.FormatMinute(2),
					new List<Photo> { PhotoInfoMinute2Second0 }
				}
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameMinute(1, 0, "-1"),
				PhotoFakes.WithNewFileNameMinute(1, 1, "-2"),
				PhotoFakes.WithNewFileNameMinute(2, 0, string.Empty),
			}
		}
	};

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> ThreeMinuteComplex = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatMinute(3),
					new List<Photo> { PhotoInfoMinute3Second17, PhotoInfoMinute3Second36, PhotoInfoMinute3Second54 }
				},
				{
					DateTimeFakes.FormatMinute(1),
					new List<Photo> { PhotoInfoMinute1Second0, PhotoInfoMinute1Second1 }
				},
				{
					DateTimeFakes.FormatMinute(2),
					new List<Photo> { PhotoInfoMinute2Second0 }
				}
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameMinute(3, 36, "-2"),
				PhotoFakes.WithNewFileNameMinute(1, 1, "-2"),
				PhotoFakes.WithNewFileNameMinute(3, 54, "-3"),
				PhotoFakes.WithNewFileNameMinute(2, 0, string.Empty),
				PhotoFakes.WithNewFileNameMinute(3, 17, "-1"),
				PhotoFakes.WithNewFileNameMinute(1, 0, "-1"),
			}
		}
	};

	[Theory]
	[MemberData(nameof(EveryMinuteHasOnePhoto))] // Every minute there is one photo, so name will be just yyyy.MM.dd-HH:mm. There is no need for number postfix.
	[MemberData(nameof(MinuteOneHasTwoPhotoDayMinuteTwoHasOnePhoto))] // Minute one has two photo, it will have number postfix after date like yyyy.MM.dd-HH:mm-{number-postfix}. Minute two has one photo there will be no postfix.
	[MemberData(nameof(ThreeMinuteComplex))] // Three minute complex scenario
	[MemberData(nameof(DateNamingStyleEmpty))] // Empty input of PhotoInfos should not break.
	public void DateTimeWithMinute_NamingStyle_Assign_New_Name_By_Grouping_On_Day_Hour_Minute(Dictionary<string, List<Photo>> mockGroupedPhotoInfosByDate,
		List<Photo> expectedPhotoInfosWithNewFileNames)
	{
		VerifyFileNamesOnGroupedByDateDictionaryWithExpected(NamingStyle.DateTimeWithMinutes, mockGroupedPhotoInfosByDate, expectedPhotoInfosWithNewFileNames);
	}

	#endregion Date Time With Minute

	#region Date Time With Second

	private static readonly Photo PhotoInfoSecond1Fake1 = PhotoFakes.WithSecond(1);
	private static readonly Photo PhotoInfoSecond1Fake2 = PhotoFakes.WithSecond(1);
	private static readonly Photo PhotoInfoSecond2Fake1 = PhotoFakes.WithSecond(2);
	private static readonly Photo PhotoInfoSecond3Fake1 = PhotoFakes.WithSecond(3);
	private static readonly Photo PhotoInfoSecond3Fake2 = PhotoFakes.WithSecond(3);
	private static readonly Photo PhotoInfoSecond3Fake3 = PhotoFakes.WithSecond(3);

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> EverySecondHasOnePhoto = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{ DateTimeFakes.FormatSecond(1), new List<Photo> { PhotoInfoSecond1Fake1 } },
				{ DateTimeFakes.FormatSecond(2), new List<Photo> { PhotoInfoSecond2Fake1 } },
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameSecond(1, string.Empty),
				PhotoFakes.WithNewFileNameSecond(2, string.Empty),
			}
		}
	};

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> SecondOneHasTwoPhotoDaySecondTwoHasOnePhoto = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatSecond(1),
					new List<Photo> { PhotoInfoSecond1Fake1, PhotoInfoSecond1Fake2 }
				},
				{
					DateTimeFakes.FormatSecond(2),
					new List<Photo> { PhotoInfoSecond2Fake1 }
				}
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameSecond(1, "-1"),
				PhotoFakes.WithNewFileNameSecond(1, "-2"),
				PhotoFakes.WithNewFileNameSecond(2, string.Empty),
			}
		}
	};

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> ThreeSecondComplex = new()
	{
		{
			new Dictionary<string, List<Photo>>
			{
				{
					DateTimeFakes.FormatSecond(3),
					new List<Photo> { PhotoInfoSecond3Fake1, PhotoInfoSecond3Fake2, PhotoInfoSecond3Fake3 }
				},
				{
					DateTimeFakes.FormatSecond(1),
					new List<Photo> { PhotoInfoSecond1Fake1, PhotoInfoSecond1Fake2 }
				},
				{
					DateTimeFakes.FormatSecond(2),
					new List<Photo> { PhotoInfoSecond2Fake1 }
				}
			},
			new List<Photo>
			{
				PhotoFakes.WithNewFileNameSecond(3, "-2"),
				PhotoFakes.WithNewFileNameSecond(1, "-1"),
				PhotoFakes.WithNewFileNameSecond(3, "-1"),
				PhotoFakes.WithNewFileNameSecond(2, string.Empty),
				PhotoFakes.WithNewFileNameSecond(3, "-3"),
				PhotoFakes.WithNewFileNameSecond(1, "-2"),
			}
		}
	};

	[Theory]
	[MemberData(nameof(EverySecondHasOnePhoto))] // Every minute there is one photo, so name will be just yyyy.MM.dd-HH:mm. There is no need for number postfix.
	[MemberData(nameof(SecondOneHasTwoPhotoDaySecondTwoHasOnePhoto))] // Minute one has two photo, it will have number postfix after date like yyyy.MM.dd-HH:mm-{number-postfix}. Minute two has one photo there will be no postfix.
	[MemberData(nameof(ThreeSecondComplex))] // Three minute complex scenario
	[MemberData(nameof(DateNamingStyleEmpty))] // Empty input of PhotoInfos should not break.
	public void DateTimeWithSecond_NamingStyle_Assign_New_Name_By_Grouping_On_Day_Hour_Minute(Dictionary<string, List<Photo>> mockGroupedPhotoInfosByDate,
		List<Photo> expectedPhotoInfosWithNewFileNames)
	{
		VerifyFileNamesOnGroupedByDateDictionaryWithExpected(NamingStyle.DateTimeWithSeconds, mockGroupedPhotoInfosByDate, expectedPhotoInfosWithNewFileNames);
	}

	#endregion Date Time With Second

	private void VerifyFileNamesOnGroupedByDateDictionaryWithExpected(NamingStyle namingStyle, Dictionary<string, List<Photo>> mockGroupedPhotoInfosByDate,
		IReadOnlyCollection<Photo> expectedPhotoInfosWithNewFileNames)
	{
		SetupSequentialNumberEnumeratorForEachPossibleNumber(expectedPhotoInfosWithNewFileNames.Count, _sequentialNumberEnumerator);


		_photoTakenDateGrouper.Setup(s => s.Group(It.IsAny<List<Photo>>(), namingStyle)).Returns(() => mockGroupedPhotoInfosByDate);

		_sut.SetFileName(
			new List<Photo>(), // dummy value : not used in DateTime based naming styles
			namingStyle,
			It.IsAny<NumberNamingTextStyle>()
		);

		_photoTakenDateGrouper.Verify(v => v.Group(It.IsAny<List<Photo>>(), namingStyle), Times.Once);
		_photoTakenDateGrouper.VerifyNoOtherCalls();

		var allPhotoInfosFlattened = mockGroupedPhotoInfosByDate.SelectMany(s => s.Value).ToList();
		allPhotoInfosFlattened.Should().BeEquivalentTo(expectedPhotoInfosWithNewFileNames);
	}

	private static void SetupSequentialNumberEnumeratorForEachPossibleNumber(int toNameUntil, Mock<ISequentialNumberEnumeratorService> sequentialNumberEnumerator)
	{
		var fakeFileNameListForLongestIterationPossible = new List<string>();
		for (var i = 1; i <= toNameUntil; i++)
			fakeFileNameListForLongestIterationPossible.Add(i.ToString());
		for (var i = 1; i <= toNameUntil; i++)
		{
			var copyIteratorValue = i;
			sequentialNumberEnumerator.Setup(s => s.NumberIterator(copyIteratorValue, It.IsAny<NumberNamingTextStyle>()))
				.Returns(() => fakeFileNameListForLongestIterationPossible);
		}
	}

	public static readonly TheoryData<Dictionary<string, List<Photo>>, List<Photo>> DateNamingStyleEmpty = new()
	{
		{
			new Dictionary<string, List<Photo>>(),
			new List<Photo>()
		}
	};

	[Fact]
	public void Not_Defined_NumberNamingTextStyle_Should_Throw_PhotoOrganizerToolException()
	{
		_sequentialNumberEnumerator.Setup(s => s.NumberIterator(It.IsAny<int>(), It.IsAny<NumberNamingTextStyle>()))
			.Throws<PhotoCliException>();

		Assert.Throws<PhotoCliException>(() => _sut.SetFileName(new List<Photo>(), NamingStyle.Numeric, (NumberNamingTextStyle)byte.MaxValue));
		_sequentialNumberEnumerator.Verify(v => v.NumberIterator(It.IsAny<int>(), It.IsAny<NumberNamingTextStyle>()), Times.Once);
		VerifyNotOtherCalls();
	}


	public static TheoryData<List<Photo>, List<Photo>> SetArchiveFileNameData = new()
	{
		{
			new List<Photo>
			{
				PhotoFakes.WithSecondAndSha1Hash(1, Sha1HashFakes.Sample(1)),
			},
			new List<Photo>
			{
				PhotoFakes.WithArchiveFileName(1, Sha1HashFakes.Sample(1)),
			}
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithSecondAndSha1Hash(2, Sha1HashFakes.Sample(2)),
				PhotoFakes.WithSecondAndSha1Hash(3, Sha1HashFakes.Sample(3)),
			},
			new List<Photo>
			{
				PhotoFakes.WithArchiveFileName(2, Sha1HashFakes.Sample(2)),
				PhotoFakes.WithArchiveFileName(3, Sha1HashFakes.Sample(3)),
			}
		},
		{
			new List<Photo>(),
			new List<Photo>()
		}
	};

	[Theory]
	[MemberData(nameof(SetArchiveFileNameData))]
	public void SetArchiveFileName_GivenPhotoInput_ShouldRenameAndMatch(List<Photo> inputPhotos, List<Photo> expectedPhotos)
	{
		_sut.SetArchiveFileName(inputPhotos);
		inputPhotos.Should().BeEquivalentTo(expectedPhotos);
	}

	private void VerifyNotOtherCalls()
	{
		_sequentialNumberEnumerator.VerifyNoOtherCalls();
		_photoTakenDateGrouper.VerifyNoOtherCalls();
	}
}
