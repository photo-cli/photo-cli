namespace PhotoCli.Tests.UnitTests.Services;

public class FolderRenamerServiceUnitTests
{
	private readonly FolderRenamerService _sut = new(ToolOptionFakes.Create(), NullLogger<FolderRenamerService>.Instance);
	private const string TargetRelativeDirectoryPath = "folder1";

	#region Renaming Folder Flows

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> FirstYearMonthDay = new()
	{
		{
			[PhotoFakes.WithDay(1)],
			FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatDay(1), TargetRelativeDirectoryPath)
		},
		{
			[
				PhotoFakes.WithDay(1),
				PhotoFakes.WithDay(1),
				PhotoFakes.WithDay(2)
			],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatDay(1))
		},
		{
			[
				PhotoFakes.WithDay(2),
				PhotoFakes.WithDay(3),
				PhotoFakes.WithDay(4),
				PhotoFakes.WithDay(4),
				PhotoFakes.NoPhotoTakenDate(),
			],
			FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatDay(2), TargetRelativeDirectoryPath)
		}
	};

	[Theory]
	[MemberData(nameof(FirstYearMonthDay))]
	public void FirstYearMonthDay_Should_Appended_To_TargetRelativePath(List<Photo> orderedPhotoInfos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(orderedPhotoInfos, FolderAppendType.FirstYearMonthDay, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> FirstYearMonth = new()
	{
		{
			[PhotoFakes.WithMonth(1)],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatMonth(1))
		},
		{
			[
				PhotoFakes.WithMonth(1),
				PhotoFakes.WithMonth(1),
				PhotoFakes.WithMonth(2),
			],
			FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatMonth(1), TargetRelativeDirectoryPath)
		},
		{
			[
				PhotoFakes.WithMonth(2),
				PhotoFakes.WithMonth(3),
				PhotoFakes.WithMonth(4),
				PhotoFakes.WithMonth(4),
				PhotoFakes.NoPhotoTakenDate(),
			],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatMonth(2))
		}
	};

	[Theory]
	[MemberData(nameof(FirstYearMonth))]
	public void FirstYearMonth_Should_Appended_To_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(photos, FolderAppendType.FirstYearMonth, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> FirstYear = new()
	{
		{
			[PhotoFakes.WithYear(2001)],
			FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatYear(2001), TargetRelativeDirectoryPath)
		},
		{
			[
				PhotoFakes.WithYear(2001),
				PhotoFakes.WithYear(2001),
				PhotoFakes.WithYear(2002),
			],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, DateTimeFakes.FormatYear(2001))
		},
		{
			[
				PhotoFakes.WithYear(2002),
				PhotoFakes.WithYear(2003),
				PhotoFakes.WithYear(2004),
				PhotoFakes.WithYear(2004),
				PhotoFakes.NoPhotoTakenDate(),
			],
			FolderAppendLocationType.Prefix,
			AppendFolder(DateTimeFakes.FormatYear(2002), TargetRelativeDirectoryPath)
		}
	};

	[Theory]
	[MemberData(nameof(FirstYear))]
	public void FirstYear_Should_Appended_To_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(photos, FolderAppendType.FirstYear, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> DayRange = new()
	{
		{
			[PhotoFakes.WithDay(1)],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, FormatDayRange(DateTimeFakes.FormatDay(1), DateTimeFakes.FormatDay(1)))
		},
		{
			[
				PhotoFakes.WithDay(1),
				PhotoFakes.WithDay(1),
				PhotoFakes.WithDay(2)
			],
			FolderAppendLocationType.Prefix,
			AppendFolder(FormatDayRange(DateTimeFakes.FormatDay(1), DateTimeFakes.FormatDay(2)), TargetRelativeDirectoryPath)
		},
		{
			[
				PhotoFakes.WithDay(1),
				PhotoFakes.WithDay(2),
				PhotoFakes.WithDay(3),
				PhotoFakes.WithDay(4),
				PhotoFakes.NoPhotoTakenDate()
			],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, FormatDayRange(DateTimeFakes.FormatDay(1), DateTimeFakes.FormatDay(4)))
		}
	};

	[Theory]
	[MemberData(nameof(DayRange))]
	public void DayRange_Should_Appended_To_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(photos, FolderAppendType.DayRange, location, expectedTargetRelativeDirectoryPath);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> MatchingMinimumAddressAllMatching = new()
	{
		{
			[PhotoFakes.WithReverseGeocodes("country")],
			FolderAppendLocationType.Prefix,
			AppendFolder("country", TargetRelativeDirectoryPath)
		},
		{
			[PhotoFakes.WithReverseGeocodes("country", "province")],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, "country", "province")
		},
		{
			[
				PhotoFakes.WithReverseGeocodes("country", "province", "town"),
				PhotoFakes.NoReverseGeocode(),
			],
			FolderAppendLocationType.Prefix,
			AppendFolder("country", "province", "town", TargetRelativeDirectoryPath)
		},
	};

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> MatchingMinimumAddressSameReverseGeocodeCountButDifferentValues = new()
	{
		{
			[
				PhotoFakes.WithReverseGeocodes("country matching", "province different 1"),
				PhotoFakes.WithReverseGeocodes("country matching", "province different 2")
			],
			FolderAppendLocationType.Prefix,
			AppendFolder("country matching", TargetRelativeDirectoryPath)
		},
		{
			[
				PhotoFakes.WithReverseGeocodes("country matching", "province matching", "town different1"),
				PhotoFakes.WithReverseGeocodes("country matching", "province matching", "town different2"),
				PhotoFakes.NoReverseGeocode()
			],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, "country matching", "province matching")
		},
	};

	public static TheoryData<List<Photo>, FolderAppendLocationType, string> MatchingMinimumAddressDifferentReverseGeocodeCounts = new()
	{
		{
			[
				PhotoFakes.WithReverseGeocodes("country matching"),
				PhotoFakes.WithReverseGeocodes("country matching", "province extra"),
			],
			FolderAppendLocationType.Suffix,
			AppendFolder(TargetRelativeDirectoryPath, "country matching")
		},
		{
			[
				PhotoFakes.WithReverseGeocodes("country matching", "province matching"),
				PhotoFakes.WithReverseGeocodes("country matching", "province matching", "town extra"),
				PhotoFakes.NoReverseGeocode()
			],
			FolderAppendLocationType.Prefix, AppendFolder("country matching", "province matching", TargetRelativeDirectoryPath)
		},
	};

	[Theory]
	[MemberData(nameof(MatchingMinimumAddressAllMatching))]
	[MemberData(nameof(MatchingMinimumAddressSameReverseGeocodeCountButDifferentValues))]
	[MemberData(nameof(MatchingMinimumAddressDifferentReverseGeocodeCounts))]
	public void MatchingMinimumAddress_Should_Appended_To_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location, string expectedTargetRelativeDirectoryPath)
	{
		Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(photos, FolderAppendType.MatchingMinimumAddress, location, expectedTargetRelativeDirectoryPath);
	}

	private void Verify_TargetRelativeDirectoryPath_Renamed_As_Expected(List<Photo> orderedPhotoInfos, FolderAppendType folderAppendType, FolderAppendLocationType folderAppendLocationType,
		string expectedTargetRelativeDirectoryPath)
	{
		var renamedPhotos = _sut.RenameByFolderAppendType(orderedPhotoInfos, folderAppendType, folderAppendLocationType, TargetRelativeDirectoryPath);
		renamedPhotos.Select(s => s.TargetRelativePath).Should().AllBe(expectedTargetRelativeDirectoryPath);
	}

	#endregion

	#region No Renaming Flows

	public static TheoryData<List<Photo>, FolderAppendLocationType> NoPhotoTakenDate = new()
	{
		{
			[PhotoFakes.NoPhotoTakenDate()],
			FolderAppendLocationType.Prefix
		},
		{
			[
				PhotoFakes.NoPhotoTakenDate(),
				PhotoFakes.NoPhotoTakenDate()
			],
			FolderAppendLocationType.Suffix
		},
	};

	[Theory]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void FirstYearMonth_With_NoPhotoTakenPhotos_Should_Not_Set_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location)
	{
		Verify_TargetRelativeDirectoryPath_Not_Set(photos, FolderAppendType.FirstYearMonth, location);
	}

	[Theory]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void FirstYear_With_NoPhotoTakenPhotos_Should_Not_Set_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location)
	{
		Verify_TargetRelativeDirectoryPath_Not_Set(photos, FolderAppendType.FirstYear, location);
	}

	[Theory]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void DayRange_With_NoPhotoTakenPhotos_Should_Not_Set_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location)
	{
		Verify_TargetRelativeDirectoryPath_Not_Set(photos, FolderAppendType.DayRange, location);
	}

	public static TheoryData<List<Photo>, FolderAppendLocationType> MatchingMinimumAddressAllDifferent = new()
	{
		{
			[
				PhotoFakes.WithReverseGeocodes("country different1"),
				PhotoFakes.WithReverseGeocodes("country different2")
			],
			FolderAppendLocationType.Suffix
		},
	};

	[Theory]
	[MemberData(nameof(MatchingMinimumAddressAllDifferent))]
	[MemberData(nameof(NoPhotoTakenDate))]
	public void MatchingMinimumAddress_With_NoPhotoTakenPhotos_Should_Not_Set_TargetRelativePath(List<Photo> photos, FolderAppendLocationType location)
	{
		Verify_TargetRelativeDirectoryPath_Not_Set(photos, FolderAppendType.MatchingMinimumAddress, location);
	}

	private void Verify_TargetRelativeDirectoryPath_Not_Set(List<Photo> orderedPhotoInfos, FolderAppendType folderAppendType, FolderAppendLocationType folderAppendLocationType)
	{
		var renamedPhotos = _sut.RenameByFolderAppendType(orderedPhotoInfos, folderAppendType, folderAppendLocationType, TargetRelativeDirectoryPath);
		renamedPhotos.Select(s => s.TargetRelativePath).Should().AllBe(null);
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
