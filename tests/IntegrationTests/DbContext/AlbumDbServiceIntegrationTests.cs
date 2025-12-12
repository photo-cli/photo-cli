using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Tests.IntegrationTests.DbContext;

public class AlbumDbServiceIntegrationTests : DbServiceIntegrationTestsBase
{
	#region New

	#region Expected Flows

	public static TheoryData<string, List<PhotoEntity>, AlbumEntity> NewIndividualAlbum = new()
	{
		{
			AlbumNameFakes.Sample(1),
			[
				PhotoEntityFakes.WithId(1),
			],
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(1), [1])
		},
		{
			AlbumNameFakes.Sample(2),
			[
				PhotoEntityFakes.WithId(2),
				PhotoEntityFakes.WithId(3),
				PhotoEntityFakes.WithId(4),
			],
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(2), [2, 3, 4])
		},
	};

	[Theory]
	[MemberData(nameof(NewIndividualAlbum))]
	public async Task NewIndividualAlbum_GivenAlbumNameAndPhotoIds_ShouldMatchWithExpectedAlbumConfigurationOnDb(string albumName, List<PhotoEntity> photoEntities, AlbumEntity expectedAlbumEntity)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();

		var actualAlbumResult = await sut.NewIndividualAlbum(albumName, photoEntities);
		actualAlbumResult.Should().Be(AlbumResult.Successful);
		await VerifyNewAlbumEntity(expectedAlbumEntity, archiveDbContextProvider);
	}

	public static TheoryData<string, AlbumDateRange, List<PhotoEntity>, AlbumEntity> NewDateRangeAlbumContainsAllPhotoTakenDate = new()
	{
		{
			AlbumNameFakes.Sample(1),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1)),
			[
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(1), 1),
			],
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1)))
		},
		{
			AlbumNameFakes.Sample(2),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2), ExifDataFakes.WithDay(4)),
			[
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(2), 2),
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(3), 3),
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(4), 4),
			],
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2), ExifDataFakes.WithDay(4)))
		},
	};

	public static TheoryData<string, AlbumDateRange, List<PhotoEntity>, AlbumEntity> NewDateRangeAlbumWithMissingPhotoTakenDate = new()
	{
		{
			AlbumNameFakes.Sample(1),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2)),
			[
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithNoPhotoTakenDate(), 1),
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(2), 2),
			],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2), ExifDataFakes.WithDay(2)), [1])
		},
		{
			AlbumNameFakes.Sample(2),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(3), ExifDataFakes.WithDay(7)),
			[
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(3), 3),
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithNoPhotoTakenDate(), 4),
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(5), 5),
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithNoPhotoTakenDate(), 6),
				PhotoEntityFakes.WithExifDataAndId(ExifDataFakes.WithDay(7), 7),
			],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(3), ExifDataFakes.WithDay(7)), [4, 6])
		},
	};

	[Theory]
	[MemberData(nameof(NewDateRangeAlbumContainsAllPhotoTakenDate))]
	[MemberData(nameof(NewDateRangeAlbumWithMissingPhotoTakenDate))]
	public async Task NewDateRangeAlbum_GivenAlbumNameAndPhotoIds_ShouldMatchWithExpectedAlbumConfigurationOnMemorySqLiteDb(string albumName, AlbumDateRange albumDateRange,
		List<PhotoEntity> photoEntities, AlbumEntity expectedAlbumEntity)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();

		var actualAlbumResult = await sut.NewDateRangeAlbum(albumName, albumDateRange, photoEntities);
		actualAlbumResult.Should().Be(AlbumResult.Successful);
		await VerifyNewAlbumEntity(expectedAlbumEntity, archiveDbContextProvider);
	}

	#endregion

	#region Breaking Flows

	[Fact]
	public async Task NewIndividualAlbum_GivenEmptyPhotos_ShouldPreventSavingAndReturnAsNoPhotosToAddInAlbum()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();

		var actualAlbumResult = await sut.NewIndividualAlbum(AlbumNameFakes.Valid(), []);
		actualAlbumResult.Should().Be(AlbumResult.NoPhotosToAddInAlbum);
		await VerifyAlbumCount(0, archiveDbContextProvider);
	}

	[Fact]
	public async Task NewIndividualAlbum_GivenExistingAlbumName_ShouldPreventSavingAndReturnAsAlbumExists()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var albumName = AlbumNameFakes.Valid();
		await AddAlbumWithName(albumName, archiveDbContextProvider);

		var actualAlbumResult = await sut.NewIndividualAlbum(albumName, [PhotoEntityFakes.WithId(1)]);
		actualAlbumResult.Should().Be(AlbumResult.AlbumExists);
		await VerifyAlbumCount(1, archiveDbContextProvider);
	}

	[Fact]
	public async Task NewDateRangeAlbum_GivenExistingAlbumName_ShouldPreventSavingAndReturnAsAlbumExists()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var albumName = AlbumNameFakes.Valid();
		await AddAlbumWithName(albumName, archiveDbContextProvider);

		var actualAlbumResult = await sut.NewDateRangeAlbum(albumName, AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1)), [PhotoEntityFakes.WithId(1)]);
		actualAlbumResult.Should().Be(AlbumResult.AlbumExists);
		await VerifyAlbumCount(1, archiveDbContextProvider);
	}

	#endregion

	#endregion

	#region Update

	#region Expecting Flows

	public static TheoryData<AlbumEntity, List<PhotoEntity>, AlbumEntity> UpdateIndividualAlbumWithPhotoIds = new()
	{
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(1), [1]),
			[
				PhotoEntityFakes.WithId(2),
			],
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(1), [1, 2])
		},
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(2), [2, 4]),
			[
				PhotoEntityFakes.WithId(3), PhotoEntityFakes.WithId(5), PhotoEntityFakes.WithId(7)
			],
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(2), [2, 4, 3, 5, 7])
		},
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(3), []),
			[
				PhotoEntityFakes.WithId(8), PhotoEntityFakes.WithId(9)
			],
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(3), [8, 9])
		},
	};

	public static TheoryData<AlbumEntity, List<PhotoEntity>, AlbumEntity> UpdateIndividualAlbumWithDateRange = new()
	{
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2))),
			[
				PhotoEntityFakes.WithId(1),
			],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2)), [1])
		},
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2001), ExifDataFakes.WithYear(2002))),
			[
				PhotoEntityFakes.WithId(2), PhotoEntityFakes.WithId(3),
			],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2001), ExifDataFakes.WithYear(2002)), [2, 3])
		},
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(3), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2001), ExifDataFakes.WithYear(2002))),
			[
				PhotoEntityFakes.WithId(2), PhotoEntityFakes.WithId(3),
			],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(3), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2001), ExifDataFakes.WithYear(2002)), [2, 3])
		},
	};

	public static TheoryData<AlbumEntity, List<PhotoEntity>, AlbumEntity> UpdateIndividualAlbumBothPhotoIdAndDateRange = new()
	{
		{
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2)), [1]),
			[
				PhotoEntityFakes.WithId(2),
			],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2)), [1, 2])
		},
		{
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithMonth(2), ExifDataFakes.WithMonth(3)), [3, 5]),
			[
				PhotoEntityFakes.WithId(4), PhotoEntityFakes.WithId(6), PhotoEntityFakes.WithId(7),
			],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithMonth(2), ExifDataFakes.WithMonth(3)), [3, 5, 4, 6, 7])
		},
	};

	[Theory]
	[MemberData(nameof(UpdateIndividualAlbumWithPhotoIds))]
	[MemberData(nameof(UpdateIndividualAlbumWithDateRange))]
	[MemberData(nameof(UpdateIndividualAlbumBothPhotoIdAndDateRange))]
	public async Task UpdateIndividualAlbum_GivenPhotoIds_ShouldMatchWithExpectedAlbumConfigurationOnDb(AlbumEntity existingAlbumEntity, List<PhotoEntity> photoEntitiesToAppend,
		AlbumEntity expectedAlbumEntity)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var existingAlbum = await AddAlbum(existingAlbumEntity, archiveDbContextProvider);
		var actualAlbumResult = await sut.UpdateIndividualAlbum(existingAlbum.Id, photoEntitiesToAppend);
		actualAlbumResult.Should().Be(AlbumResult.Successful);
		await VerifyUpdatedAlbumEntity(expectedAlbumEntity, archiveDbContextProvider);
	}

	public static TheoryData<AlbumEntity, AlbumDateRange, AlbumEntity> UpdateDateRangeAlbumWithPhotoIds = new()
	{
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(1), [1]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2)),
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2)), [1])
		},
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(2), [2, 4]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithMonth(1), ExifDataFakes.WithMonth(2)),
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithMonth(1), ExifDataFakes.WithMonth(2)), [2, 4])
		},
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(3), []),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2001)),
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(3), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2001)), [])
		},
	};

	public static TheoryData<AlbumEntity, AlbumDateRange, AlbumEntity> UpdateDateRangeAlbumWithoutOverlappingDateRanges = new()
	{
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2))), // existing range earlier
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(3), ExifDataFakes.WithDay(4)), // new range later
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(4))) // merged
		},
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2003), ExifDataFakes.WithYear(2004))), // existing range later
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2001)), // new range earlier
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2004))) // merged
		},
	};

	public static TheoryData<AlbumEntity, AlbumDateRange, AlbumEntity> UpdateDateRangeAlbumWithOverlappingDateRanges = new()
	{
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(3))), // existing start date earlier
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2), ExifDataFakes.WithDay(4)), // new end date later
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(4))) // merged
		},
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2002), ExifDataFakes.WithYear(2004))), // existing end later
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2003)), // new start earlier
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2004))) // merged
		},
	};

	public static TheoryData<AlbumEntity, AlbumDateRange, AlbumEntity> UpdateDateRangeAlbumBothPhotoIdAndDateRange = new()
	{
		{
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(3)), [1]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2), ExifDataFakes.WithDay(4)),
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(4)), [1])
		},
		{
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2001), ExifDataFakes.WithYear(2004)), [2, 4, 6]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2002)),
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2004)), [2, 4, 6])
		},
	};

	[Theory]
	[MemberData(nameof(UpdateDateRangeAlbumWithPhotoIds))]
	[MemberData(nameof(UpdateDateRangeAlbumWithoutOverlappingDateRanges))]
	[MemberData(nameof(UpdateDateRangeAlbumWithOverlappingDateRanges))]
	[MemberData(nameof(UpdateDateRangeAlbumBothPhotoIdAndDateRange))]
	public async Task UpdateDateRangeAlbum_GivenDateRangeWithAllPhotoTakenDate_ShouldMatchWithExpectedAlbumConfigurationOnDb(AlbumEntity existingAlbumEntity, AlbumDateRange albumDateRangeNewToMerge,
		AlbumEntity expectedAlbumEntity)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var existingAlbum = await AddAlbum(existingAlbumEntity, archiveDbContextProvider);
		var actualAlbumResult = await sut.UpdateDateRangeAlbum(existingAlbum.Id, albumDateRangeNewToMerge, PhotoEntityFakes.ValidListWithAllPhotoTakenDate());
		actualAlbumResult.Should().Be(AlbumResult.Successful);
		await VerifyUpdatedAlbumEntity(expectedAlbumEntity, archiveDbContextProvider);
	}

	public static TheoryData<AlbumEntity, AlbumDateRange, List<PhotoEntity>, AlbumEntity> UpdateDateRangeAlbumWithPhotoIdsByPhotosWithoutTakenDate = new()
	{
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(1), [1]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2)),
			[PhotoEntityFakes.WithoutPhotoTakenDateAndId(2)],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(2)), [1, 2])
		},
		{
			AlbumEntityFakes.WithPhotoIdsAndName(AlbumNameFakes.Sample(2), [2, 4]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2001)),
			[PhotoEntityFakes.WithoutPhotoTakenDateAndId(3), PhotoEntityFakes.WithoutPhotoTakenDateAndId(5)],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2001)), [2, 4, 3, 5])
		},
	};

	public static TheoryData<AlbumEntity, AlbumDateRange, List<PhotoEntity>, AlbumEntity> UpdateDateRangeAlbumWithDateRangesByPhotosWithoutTakenDate = new()
	{
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(3))),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2), ExifDataFakes.WithDay(4)),
			[PhotoEntityFakes.WithoutPhotoTakenDateAndId(1)],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(4)), [1])
		},
		{
			AlbumEntityFakes.WithDateRangeAndName(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2002), ExifDataFakes.WithYear(2004))),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2003)),
			[PhotoEntityFakes.WithoutPhotoTakenDateAndId(2), PhotoEntityFakes.WithoutPhotoTakenDateAndId(4)],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2004)), [2, 4])
		},
	};

	public static TheoryData<AlbumEntity, AlbumDateRange, List<PhotoEntity>, AlbumEntity> UpdateDateRangeAlbumWithBothPhotoIdAndDateRangeByPhotosWithoutTakenDate = new()
	{
		{
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(3)), [1]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(2), ExifDataFakes.WithDay(4)),
			[PhotoEntityFakes.WithoutPhotoTakenDateAndId(2)],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(1), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithDay(1), ExifDataFakes.WithDay(4)), [1, 2])
		},
		{
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2001), ExifDataFakes.WithYear(2004)), [2, 4]),
			AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2002)),
			[PhotoEntityFakes.WithoutPhotoTakenDateAndId(5), PhotoEntityFakes.WithoutPhotoTakenDateAndId(7)],
			AlbumEntityFakes.WithDateRangeAndPhotoIds(AlbumNameFakes.Sample(2), AlbumDateRangeFakes.ByExifData(ExifDataFakes.WithYear(2000), ExifDataFakes.WithYear(2004)), [2, 4, 5, 7])
		},
	};

	[Theory]
	[MemberData(nameof(UpdateDateRangeAlbumWithPhotoIdsByPhotosWithoutTakenDate))]
	[MemberData(nameof(UpdateDateRangeAlbumWithDateRangesByPhotosWithoutTakenDate))]
	[MemberData(nameof(UpdateDateRangeAlbumWithBothPhotoIdAndDateRangeByPhotosWithoutTakenDate))]
	public async Task UpdateDateRangeAlbum_GivenDateRangeWithMissingPhotoTakenDate_ShouldMatchWithExpectedAlbumConfigurationOnDb(AlbumEntity existingAlbumEntity,
		AlbumDateRange albumDateRangeNewToMerge,
		List<PhotoEntity> photoEntities, AlbumEntity expectedAlbumEntity)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var existingAlbum = await AddAlbum(existingAlbumEntity, archiveDbContextProvider);
		var actualAlbumResult = await sut.UpdateDateRangeAlbum(existingAlbum.Id, albumDateRangeNewToMerge, photoEntities);
		actualAlbumResult.Should().Be(AlbumResult.Successful);
		await VerifyUpdatedAlbumEntity(expectedAlbumEntity, archiveDbContextProvider);
	}

	#endregion

	#region Breaking Flows

	[Fact]
	public async Task UpdateIndividualAlbum_GivenEmptyPhotos_ShouldPreventSavingAndReturnAsNoPhotosToAddInAlbum()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var actualAlbumResult = await sut.UpdateIndividualAlbum(1, []);
		actualAlbumResult.Should().Be(AlbumResult.NoPhotosToAddInAlbum);
		await VerifyAlbumCount(0, archiveDbContextProvider);
	}

	[Fact]
	public async Task UpdateIndividualAlbum_GivenExistingAlbumName_ShouldPreventSavingAndReturnAsNoPhotosToAddInAlbum()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var actualAlbumResult = await sut.UpdateIndividualAlbum(1, []);
		actualAlbumResult.Should().Be(AlbumResult.NoPhotosToAddInAlbum);
		await VerifyAlbumCount(0, archiveDbContextProvider);
	}

	[Fact]
	public async Task UpdateDateRangeAlbum_GivenNotExistingAlbumName_ShouldPreventSavingAndReturnAsAlbumNotFound()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var actualAlbumResult = await sut.UpdateDateRangeAlbum(1, AlbumDateRangeFakes.Valid(), PhotoEntityFakes.ValidList());
		actualAlbumResult.Should().Be(AlbumResult.AlbumNotFound);
		await VerifyAlbumCount(0, archiveDbContextProvider);
	}

	public static TheoryData<string> IncorrectAlbumConfigurations = new()
	{
		"{", // Missing }
		"{}_", // Invalid character at end
		"{PhotoIds\":[1]}", // Missing " before PhotoIds
		"{\"DateRange\":\"Start\":\"2023-06-10T14:40:14\",\"End\":\"2023-06-10T19:30:38\"}}" // Missing { before Start
	};

	[Theory]
	[MemberData(nameof(IncorrectAlbumConfigurations))]
	public async Task UpdateIndividualAlbum_ExistingAlbumConfigurationNotInCorrectFormat_ShouldPreventSavingAndReturnAsExistingConfigurationNotInCorrectFormat(string existingAlbumConfiguration)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var existingAlbum = await AddAlbumWithRawConfiguration(existingAlbumConfiguration, archiveDbContextProvider);
		var actualAlbumResult = await sut.UpdateIndividualAlbum(existingAlbum.Id, PhotoEntityFakes.ValidList());
		actualAlbumResult.Should().Be(AlbumResult.ExistingConfigurationNotInCorrectFormat);
	}

	[Theory]
	[MemberData(nameof(IncorrectAlbumConfigurations))]
	public async Task UpdateDateRangeAlbum_ExistingAlbumConfigurationNotInCorrectFormat_ShouldPreventSavingAndReturnAsExistingConfigurationNotInCorrectFormat(string existingAlbumConfiguration)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var existingAlbum = await AddAlbumWithRawConfiguration(existingAlbumConfiguration, archiveDbContextProvider);
		var actualAlbumResult = await sut.UpdateDateRangeAlbum(existingAlbum.Id, AlbumDateRangeFakes.Valid(), PhotoEntityFakes.ValidList());
		actualAlbumResult.Should().Be(AlbumResult.ExistingConfigurationNotInCorrectFormat);
	}

	#endregion

	#endregion

	#region AutoAddress

	public static TheoryData<List<PhotoEntity>, List<AlbumEntity>> WithReverseGeocodes = new()
	{
		{
			[
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country1", "City1", "Neighbourhood1"))
			],
			[
				AlbumEntityFakes.WithReverseGeocodeAndName("Country1-City1-Neighbourhood1", new AlbumReverseGeocode("Country1-City1-Neighbourhood1")),
				AlbumEntityFakes.WithReverseGeocodeAndName("Country1", new AlbumReverseGeocode(Address1: "Country1")),
				AlbumEntityFakes.WithReverseGeocodeAndName("City1", new AlbumReverseGeocode(Address2: "City1")),
				AlbumEntityFakes.WithReverseGeocodeAndName("Neighbourhood1", new AlbumReverseGeocode(Address3: "Neighbourhood1"))
			]
		},
		{
			[
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country2", "City2", "Neighbourhood2")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country2", "City3", "Neighbourhood3")),
			],
			[
				AlbumEntityFakes.WithReverseGeocodeAndName("Country2-City2-Neighbourhood2", new AlbumReverseGeocode("Country2-City2-Neighbourhood2")),
				AlbumEntityFakes.WithReverseGeocodeAndName("Country2-City3-Neighbourhood3", new AlbumReverseGeocode("Country2-City3-Neighbourhood3")),
				AlbumEntityFakes.WithReverseGeocodeAndName("Country2", new AlbumReverseGeocode(Address1: "Country2")),
				AlbumEntityFakes.WithReverseGeocodeAndName("City2", new AlbumReverseGeocode(Address2: "City2")),
				AlbumEntityFakes.WithReverseGeocodeAndName("City3", new AlbumReverseGeocode(Address2: "City3")),
				AlbumEntityFakes.WithReverseGeocodeAndName("Neighbourhood2", new AlbumReverseGeocode(Address3: "Neighbourhood2")),
				AlbumEntityFakes.WithReverseGeocodeAndName("Neighbourhood3", new AlbumReverseGeocode(Address3: "Neighbourhood3"))
			]
		},
	};

	public static TheoryData<List<PhotoEntity>, List<AlbumEntity>> WithoutReverseGeocodes = new()
	{
		{ [], [] },
		{ [PhotoEntityFakes.WithoutReverseGeocodes()], [] },
	};

	[Theory]
	[MemberData(nameof(WithReverseGeocodes))]
	[MemberData(nameof(WithoutReverseGeocodes))]
	public async Task SaveReverseGeocodeAlbums_GivenPhotosWithReverseGeocodes_ShouldMatchWithExpectedAlbumConfigurationOnDb(List<PhotoEntity> photoEntities, List<AlbumEntity> expectedAlbumEntities)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var actualAlbumResult = await sut.SaveReverseGeocodeAlbums(photoEntities);
		actualAlbumResult.Should().Be(AlbumResult.Successful);
		await VerifyAlbumEntities(expectedAlbumEntities, archiveDbContextProvider);
	}

	#endregion

	#region GetAlbumByName

	public static TheoryData<AlbumEntity[], string, AlbumEntity?> GetAlbumByNameWithVariousDbStates = new()
	{
		{
			[], AlbumNameFakes.Sample(1), null
		},
		{
			[AlbumEntityFakes.Sample(2)], AlbumNameFakes.Sample(2), AlbumEntityFakes.Sample(2)
		},
		{
			[AlbumEntityFakes.Sample(3), AlbumEntityFakes.Sample(4), AlbumEntityFakes.Sample(5)], AlbumNameFakes.Sample(4), AlbumEntityFakes.Sample(4)
		},
		{
			[AlbumEntityFakes.Sample(5), AlbumEntityFakes.Sample(6), AlbumEntityFakes.Sample(7)], AlbumNameFakes.Sample(8), null
		},
	};

	[Theory]
	[MemberData(nameof(GetAlbumByNameWithVariousDbStates))]
	public async Task GetAlbumByName_GivenExistingAlbumsStateInDbContext_ShouldReturnMatchingAlbum(AlbumEntity[] existingAlbums, string requestingAlbumName, AlbumEntity? expectedAlbum)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);
		var actualAlbum = await sut.GetAlbumByName(requestingAlbumName);

		actualAlbum.Should().BeEquivalentTo(expectedAlbum, c => c
			.Excluding(e => e!.Id)
			.Excluding(e => e!.CreatedAt)
		);
	}

	[Fact]
	public async Task GetAlbumByName_GivenDuplicateAlbumNames_ShouldThrowInvalidOperationException()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		await AddAlbumWithName(AlbumNameFakes.Sample(1), archiveDbContextProvider);
		await AddAlbumWithName(AlbumNameFakes.Sample(1), archiveDbContextProvider);
		await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetAlbumByName(AlbumNameFakes.Sample(1)));
	}

	#endregion

	#region GetAlbumById

	public static TheoryData<AlbumEntity[], int, AlbumEntity?> GetAlbumByIdWithVariousDbStates = new()
	{
		{
			[], 1, null
		},
		{
			[AlbumEntityFakes.WithSpecificId(2)], 2, AlbumEntityFakes.WithSpecificId(2)
		},
		{
			[AlbumEntityFakes.WithSpecificId(3), AlbumEntityFakes.WithSpecificId(4), AlbumEntityFakes.WithSpecificId(5)], 4, AlbumEntityFakes.WithSpecificId(4)
		},
		{
			[AlbumEntityFakes.WithSpecificId(5), AlbumEntityFakes.WithSpecificId(6), AlbumEntityFakes.WithSpecificId(7)], 8, null
		},
	};

	[Theory]
	[MemberData(nameof(GetAlbumByIdWithVariousDbStates))]
	public async Task GetAlbumById_GivenExistingAlbumsStateInDbContext_ShouldReturnMatchingAlbum(AlbumEntity[] existingAlbums, int requestingAlbumId, AlbumEntity? expectedAlbum)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);
		var actualAlbum = await sut.GetAlbumById(requestingAlbumId);

		actualAlbum.Should().BeEquivalentTo(expectedAlbum, c => c
			.Excluding(e => e!.Id)
			.Excluding(e => e!.CreatedAt)
		);
	}


	[Fact]
	public async Task GetAlbumById_GivenDuplicateAlbumNames_ShouldThrowInvalidOperationException()
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		await AddAlbumWithName(AlbumNameFakes.Sample(1), archiveDbContextProvider);
		await AddAlbumWithName(AlbumNameFakes.Sample(1), archiveDbContextProvider);
		await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetAlbumByName(AlbumNameFakes.Sample(1)));
	}

	#endregion

	#region GetAllAlbums

	public static TheoryData<AlbumEntity[], AlbumEntity[]> GetAllAlbumsWithVariousDbStates = new()
	{
		{
			[],
			[]
		},
		{
			[AlbumEntityFakes.Sample(1)],
			[AlbumEntityFakes.Sample(1)]
		},
		{
			[
				AlbumEntityFakes.Sample(2),
				AlbumEntityFakes.Deleted(3),
				AlbumEntityFakes.Sample(4)
			],
			[
				AlbumEntityFakes.Sample(2),
				AlbumEntityFakes.Sample(4)
			]
		},
	};

	[Theory]
	[MemberData(nameof(GetAllAlbumsWithVariousDbStates))]
	public async Task GetAllAlbums_GivenExistingAlbumsStateInDbContext_ShouldReturnMatchingAlbum(AlbumEntity[] existingAlbums, AlbumEntity[] expectedAlbums)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);

		var actualAlbums = await sut.GetAllAlbums();

		actualAlbums.Should().BeEquivalentTo(expectedAlbums, c => c
			.Excluding(e => e!.Id)
			.Excluding(e => e!.CreatedAt)
		);
	}

	#endregion

	#region GetAllAlbums

	public static TheoryData<AlbumEntity[], int> TotalAlbumCountWithVariousDbStates = new()
	{
		{
			[],
			0
		},
		{
			[
				AlbumEntityFakes.Sample(1)
			],
			1
		},
		{
			[
				AlbumEntityFakes.Sample(2),
				AlbumEntityFakes.Deleted(3),
				AlbumEntityFakes.Sample(4)
			],
			2
		},
	};

	[Theory]
	[MemberData(nameof(TotalAlbumCountWithVariousDbStates))]
	public async Task TotalAlbumCount_GivenExistingAlbumsStateInDbContext_ShouldReturnCorrectCount(AlbumEntity[] existingAlbums, int expectedAlbumCount)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);
		var actualAlbumCount = await sut.TotalAlbumCount();
		actualAlbumCount.Should().Be(expectedAlbumCount);
	}

	#endregion

	#region Helpers

	private static Task<AlbumEntity> AddAlbumWithName(string albumName, ArchiveDbContextProvider archiveDbContextProvider)
	{
		var albumEntity = new AlbumEntity(albumName, AlbumType.UserDefined, DateTime.Now) { Configuration = "{}" };
		return AddAlbum(albumEntity, archiveDbContextProvider);
	}

	private static Task<AlbumEntity> AddAlbumWithRawConfiguration(string rawConfiguration, ArchiveDbContextProvider archiveDbContextProvider)
	{
		return AddAlbum(AlbumEntityFakes.WithRawConfiguration(rawConfiguration), archiveDbContextProvider);
	}

	private static Task VerifyNewAlbumEntity(AlbumEntity expectedAlbumEntity, ArchiveDbContextProvider archiveDbContextProvider)
	{
		return VerifyAlbumEntity(expectedAlbumEntity, archiveDbContextProvider, false);
	}

	private static Task VerifyUpdatedAlbumEntity(AlbumEntity expectedAlbumEntity, ArchiveDbContextProvider archiveDbContextProvider)
	{
		return VerifyAlbumEntity(expectedAlbumEntity, archiveDbContextProvider, true);
	}

	private static async Task VerifyAlbumEntity(AlbumEntity expectedAlbumEntity, ArchiveDbContextProvider archiveDbContextProvider, bool verifyModifiedAt)
	{
		var albumEntities = await archiveDbContextProvider.CreateOrGetInstance().Albums.ToListAsync();
		using (new AssertionScope())
		{
			albumEntities.Count.Should().Be(1);
			var actualAlbumEntity = albumEntities.Single();
			actualAlbumEntity.Id.Should().BePositive();
			actualAlbumEntity.CreatedAt.Should().BeAfter(DateTime.Today);

			if (verifyModifiedAt)
				actualAlbumEntity.ModifiedAt.Should().BeAfter(DateTime.Today);
			else
				actualAlbumEntity.ModifiedAt.Should().BeNull();

			actualAlbumEntity.Should().BeEquivalentTo(expectedAlbumEntity, c => c
				.Excluding(e => e.Id)
				.Excluding(e => e.CreatedAt)
				.Excluding(e => e.ModifiedAt)
				.Excluding(e => e.History)
			);

			var albumHistoryEntities = await archiveDbContextProvider.CreateOrGetInstance().AlbumHistories.ToListAsync();
			albumHistoryEntities.Count.Should().Be(1);
			var actualAlbumHistoryEntity = albumHistoryEntities.Single();
			actualAlbumHistoryEntity.Id.Should().BePositive();
			actualAlbumHistoryEntity.Album?.Id.Should().Be(actualAlbumEntity.Id);
			actualAlbumHistoryEntity.Configuration.Should().Be(expectedAlbumEntity.Configuration);
			actualAlbumHistoryEntity.SnapshotAt.Should().BeAfter(DateTime.Today);
		}
	}

	private static async Task VerifyAlbumEntities(List<AlbumEntity> expectedAlbumEntities, ArchiveDbContextProvider archiveDbContextProvider)
	{
		var albumEntities = await archiveDbContextProvider.CreateOrGetInstance().Albums.ToListAsync();
		expectedAlbumEntities.Should().BeEquivalentTo(albumEntities, c => c
			.Excluding(e => e.Id)
			.Excluding(e => e.CreatedAt)
			.Excluding(e => e.ModifiedAt)
			.Excluding(e => e.History)
		);
	}

	private static async Task VerifyAlbumCount(int expected, ArchiveDbContextProvider archiveDbContextProvider)
	{
		var albumEntities = await archiveDbContextProvider.CreateOrGetInstance().Albums.ToListAsync();
		albumEntities.Count.Should().Be(expected);
	}

	#endregion
}
