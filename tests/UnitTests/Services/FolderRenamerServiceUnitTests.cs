namespace PhotoCli.Tests.UnitTests.Services;

public class FolderRenamerServiceUnitTests
{
	private readonly FolderRenamerService _sut = new(ToolOptionFakes.Create(), NullLogger<FolderRenamerService>.Instance);
	private const string TargetRelativeDirectoryPath = "folder1";

	#region Process

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> FirstYearMonthDay = new()
	{
		{ new List<Photo> { PhotoFakes.WithDay(1) }, FolderAppendLocationType.Prefix, AppendFolder(DateTimeFakes.FormatDay(1), TargetRelativeDirectoryPath) },
		{
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(1), PhotoFakes.WithDay(2) }, FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatDay(1))
		},
		{
			new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(3), PhotoFakes.WithDay(4), PhotoFakes.WithDay(4), PhotoFakes.NoPhotoTakenDate() }, FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatDay(2), TargetRelativeDirectoryPath)
		}
	};

	[Theory]
	[MemberData(nameof(FirstYearMonthDay))]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void FirstYearMonthDay_Should_Appended_To_TargetRelativePath(List<Photo> orderedPhotoInfos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(orderedPhotoInfos, FolderAppendType.FirstYearMonthDay, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> FirstYearMonth = new()
	{
		{ new List<Photo> { PhotoFakes.WithMonth(1) }, FolderAppendLocationType.Suffix, AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatMonth(1)) },
		{
			new List<Photo> { PhotoFakes.WithMonth(1), PhotoFakes.WithMonth(1), PhotoFakes.WithMonth(2) }, FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatMonth(1), TargetRelativeDirectoryPath)
		},
		{
			new List<Photo> { PhotoFakes.WithMonth(2), PhotoFakes.WithMonth(3), PhotoFakes.WithMonth(4), PhotoFakes.WithMonth(4), PhotoFakes.NoPhotoTakenDate() }, FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatMonth(2))
		}
	};

	[Theory]
	[MemberData(nameof(FirstYearMonth))]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void FirstYearMonth_Should_Appended_To_TargetRelativePath(List<Photo> orderedPhotoInfos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(orderedPhotoInfos, FolderAppendType.FirstYearMonth, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> FirstYear = new()
	{
		{ new List<Photo> { PhotoFakes.WithYear(2001) }, FolderAppendLocationType.Prefix, AppendFolder(DateTimeFakes.FormatYear(2001), TargetRelativeDirectoryPath) },
		{
			new List<Photo> { PhotoFakes.WithYear(2001), PhotoFakes.WithYear(2001), PhotoFakes.WithYear(2002) }, FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatYear(2001))
		},
		{
			new List<Photo> { PhotoFakes.WithYear(2002), PhotoFakes.WithYear(2003), PhotoFakes.WithYear(2004), PhotoFakes.WithYear(2004), PhotoFakes.NoPhotoTakenDate() },
			FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatYear(2002), TargetRelativeDirectoryPath)
		}
	};

	[Theory]
	[MemberData(nameof(FirstYear))]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void FirstYear_Should_Appended_To_TargetRelativePath(List<Photo> orderedPhotoInfos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(orderedPhotoInfos, FolderAppendType.FirstYear, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> DayRange = new()
	{
		{
			new List<Photo> { PhotoFakes.WithDay(1) }, FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, FormatDayRange(DateTimeFakes.FormatDay(1), DateTimeFakes.FormatDay(1)))
		},
		{
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(1), PhotoFakes.WithDay(2) }, FolderAppendLocationType.Prefix,
			AppendFolder(FormatDayRange(DateTimeFakes.FormatDay(1), DateTimeFakes.FormatDay(2)), TargetRelativeDirectoryPath)
		},
		{
			new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(2), PhotoFakes.WithDay(3), PhotoFakes.WithDay(4), PhotoFakes.NoPhotoTakenDate() }, FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, FormatDayRange(DateTimeFakes.FormatDay(1), DateTimeFakes.FormatDay(4)))
		}
	};

	[Theory]
	[MemberData(nameof(DayRange))]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void DayRange_Should_Appended_To_TargetRelativePath(List<Photo> orderedPhotoInfos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(orderedPhotoInfos, FolderAppendType.DayRange, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> MatchingMinimumAddressAllMatching = new()
	{
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodes("country") }, FolderAppendLocationType.Prefix,
			AppendFolder("country", TargetRelativeDirectoryPath)
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodes("country", "province") }, FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, "country", "province")
		},
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodes("country", "province", "town"), PhotoFakes.NoReverseGeocode() }, FolderAppendLocationType.Prefix,
			AppendFolder("country", "province", "town", TargetRelativeDirectoryPath)
		},
	};

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> MatchingMinimumAddressSameReverseGeocodeCountButDifferentValues = new()
	{
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodes("country matching", "province different 1"), PhotoFakes.WithReverseGeocodes("country matching", "province different 2") },
			FolderAppendLocationType.Prefix, AppendFolder("country matching", TargetRelativeDirectoryPath)
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithReverseGeocodes("country matching", "province matching", "town different1"),
				PhotoFakes.WithReverseGeocodes("country matching", "province matching", "town different2"),
				PhotoFakes.NoReverseGeocode()
			},
			FolderAppendLocationType.Suffix, AppendFolder(TargetRelativeDirectoryPath, "country matching", "province matching")
		},
	};

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> MatchingMinimumAddressDifferentReverseGeocodeCounts = new()
	{
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodes("country matching"), PhotoFakes.WithReverseGeocodes("country matching", "province extra") }, FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, "country matching")
		},
		{
			new List<Photo>
			{
				PhotoFakes.WithReverseGeocodes("country matching", "province matching"),
				PhotoFakes.WithReverseGeocodes("country matching", "province matching", "town extra"),
				PhotoFakes.NoReverseGeocode()
			},
			FolderAppendLocationType.Prefix, AppendFolder("country matching", "province matching", TargetRelativeDirectoryPath)
		},
	};

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> MatchingMinimumAddressAllDifferent = new()
	{
		{
			new List<Photo> { PhotoFakes.WithReverseGeocodes("country different1"), PhotoFakes.WithReverseGeocodes("country different2") }, FolderAppendLocationType.Suffix,
			TargetRelativeDirectoryPath
		},
	};

	[Theory]
	[MemberData(nameof(MatchingMinimumAddressAllMatching))]
	[MemberData(nameof(MatchingMinimumAddressSameReverseGeocodeCountButDifferentValues))]
	[MemberData(nameof(MatchingMinimumAddressDifferentReverseGeocodeCounts))]
	[MemberData(nameof(MatchingMinimumAddressAllDifferent))]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void MatchingMinimumAddress_Should_Appended_To_TargetRelativePath(List<Photo> orderedPhotoInfos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(orderedPhotoInfos, FolderAppendType.MatchingMinimumAddress, location, expectedTargetRelativeDirectoryPath);
	}

	private void Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(List<Photo> orderedPhotoInfos, FolderAppendType folderAppendType, FolderAppendLocationType folderAppendLocationType,
		string expectedTargetRelativeDirectoryPath)
	{
		foreach (var orderedPhotoInfo in orderedPhotoInfos)
			orderedPhotoInfo.TargetRelativeDirectoryPath = TargetRelativeDirectoryPath;
		_sut.RenameByFolderAppendType(orderedPhotoInfos, folderAppendType, folderAppendLocationType, TargetRelativeDirectoryPath);
		orderedPhotoInfos.Select(s => s.TargetRelativeDirectoryPath).Should().AllBe(expectedTargetRelativeDirectoryPath);
	}

	#endregion

	#region No Process

	public static TheoryData<List<Photo>> UnsortedPhotoInfosShouldThrowPhotoOrganizerToolExceptionData = new()
	{
		new List<Photo> { PhotoFakes.WithDay(2), PhotoFakes.WithDay(1) },
		new List<Photo> { PhotoFakes.WithDay(1), PhotoFakes.WithDay(3), PhotoFakes.WithDay(2) }
	};

	[Theory]
	[MemberData(nameof(UnsortedPhotoInfosShouldThrowPhotoOrganizerToolExceptionData))]
	public void Unsorted_PhotoInfos_Should_Throw_PhotoOrganizerToolException(List<Photo> photoInfosNotInOrder)
	{
		Assert.Throws<PhotoCliException>(() =>
		{
			_sut.RenameByFolderAppendType(photoInfosNotInOrder, FolderAppendTypeFakes.Valid(), FolderAppendLocationTypeFakes.Valid(), TargetRelativeDirectoryPath);
		});
	}

	[Fact]
	public void PhotoInfos_With_No_Date_Should_Not_Processed_And_TargetRelativeDirectoryPath_Should_Not_Changed()
	{
		var photoInfos = new List<Photo>
		{
			PhotoFakes.NoDateWithTargetRelativeDirectoryPath(TargetRelativeDirectoryPath),
			PhotoFakes.NoDateWithTargetRelativeDirectoryPath(TargetRelativeDirectoryPath)
		};
		Not_Processed_TargetRelativeDirectoryPath_Should_Not_Changed(FolderAppendType.FirstYear, photoInfos, TargetRelativeDirectoryPath);
	}

	[Fact]
	public void Empty_Root_TargetRelativeDirectoryPath_Should_Not_Processed_And_TargetRelativeDirectoryPath_Should_Not_Changed()
	{
		var targetRelativeDirectoryPath = string.Empty;
		var photoInfos = new List<Photo>
		{
			PhotoFakes.WithDayAndTargetRelativeDirectoryPath(1, targetRelativeDirectoryPath),
			PhotoFakes.WithDayAndTargetRelativeDirectoryPath(2, targetRelativeDirectoryPath)
		};
		Not_Processed_TargetRelativeDirectoryPath_Should_Not_Changed(FolderAppendType.FirstYear, photoInfos, targetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> NoPhotoTakenDate = new()
	{
		{ new List<Photo> { PhotoFakes.NoPhotoTakenDate() }, FolderAppendLocationType.Prefix, TargetRelativeDirectoryPath },
		{ new List<Photo> { PhotoFakes.NoPhotoTakenDate(), PhotoFakes.NoPhotoTakenDate() }, FolderAppendLocationType.Suffix, TargetRelativeDirectoryPath },
	};

	private void Not_Processed_TargetRelativeDirectoryPath_Should_Not_Changed(FolderAppendType folderAppendType, IReadOnlyCollection<Photo> photoInfos, string targetRelativeDirectoryPath)
	{
		_sut.RenameByFolderAppendType(photoInfos, folderAppendType, FolderAppendLocationTypeFakes.Valid(), targetRelativeDirectoryPath);
		photoInfos.Select(s => s.TargetRelativeDirectoryPath).Should().AllBe(targetRelativeDirectoryPath);
	}

	#endregion

	#region Helpers

	private static string AppendFolder(params string[] folders)
	{
		return string.Join(ToolOptionFakes.FolderAppendSeparator, folders);
	}

	private static string FormatDayRange(string start, string end)
	{
		return $"{start}{ToolOptionFakes.DayRangeSeparator}{end}";
	}

	#endregion
}
