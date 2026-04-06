namespace PhotoCli.Tests.IntegrationTests.DbContext;

public class ListDbServiceIntegrationTests : DbServiceIntegrationTestsBase
{
	#region GetAlbumPhotosById

	#region Expected Flows

	public static TheoryData<PhotoEntity[], AlbumEntity[], int, PhotoEntity[]> AlbumWithPhotoIdsConfigurationWithMatchingResult = new()
	{
		{
			[
				PhotoEntityFakes.WithId(1),
			],
			[AlbumEntityFakes.WithPhotoIdsAndId(1, [1])],
			1,
			[
				PhotoEntityFakes.WithId(1),
			]
		},
		{
			[
				PhotoEntityFakes.WithId(2),
				PhotoEntityFakes.WithId(3),
				PhotoEntityFakes.WithId(4),
				PhotoEntityFakes.WithId(5),
			],
			[AlbumEntityFakes.WithPhotoIdsAndId(2, [3, 4])],
			2,
			[
				PhotoEntityFakes.WithId(3),
				PhotoEntityFakes.WithId(4),
			]
		},
	};

	public static TheoryData<PhotoEntity[], AlbumEntity[], int, PhotoEntity[]> AlbumWithDateRangeConfigurationWithMatchingResult = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithYear(2001)),
			],
			[AlbumEntityFakes.WithDateRangeAndId(1, new AlbumDateRange(DateTimeFakes.WithYear(2000), DateTimeFakes.WithYear(2002)))],
			1,
			[
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithYear(2001)),
			]
		},
		{
			[
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(1)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(2)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(3)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(4)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(5)),
			],
			[AlbumEntityFakes.WithDateRangeAndId(2, new AlbumDateRange(DateTimeFakes.WithMonth(2), DateTimeFakes.WithMonth(4)))],
			2,
			[
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(2)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(3)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithMonth(4)),
			]
		},
		{
			[
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithDay(1)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithDay(2)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithDay(3)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithDay(4)),
			],
			[AlbumEntityFakes.WithDateRangeAndId(3, new AlbumDateRange(DateTimeFakes.WithDay(2), DateTimeFakes.WithDay(3)))],
			3,
			[
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithDay(2)),
				PhotoEntityFakes.WithPhotoTakenDate(DateTimeFakes.WithDay(3)),
			]
		},
	};

	public static TheoryData<PhotoEntity[], AlbumEntity[], int, PhotoEntity[]> AlbumWithReserveGeocodeConfigurationWithMatchingResult = new()
	{
		{
			[
				PhotoEntityFakes.WithReverseGeocodes([ReverseGeocodeFakes.SampleSingleAddress(1)]),
			],
			[AlbumEntityFakes.WithReverseGeocodeAndId(1, new AlbumReverseGeocode(Address1: ReverseGeocodeFakes.SampleSingleAddress(1)))],
			1,
			[
				PhotoEntityFakes.WithReverseGeocodes([ReverseGeocodeFakes.SampleSingleAddress(1)]),
			]
		},
		{
			[
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country1", "City1", "Neighbourhood1")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country2", "City3", "Neighbourhood3")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country1", "City2", "Neighbourhood2")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country3", "City4", "Neighbourhood5")),
			],
			[AlbumEntityFakes.WithReverseGeocodeAndId(1, new AlbumReverseGeocode(Address1: "Country1"))],
			1,
			[
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country1", "City1", "Neighbourhood1")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country1", "City2", "Neighbourhood2")),
			]
		},
		{
			[
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country4", "City5", "Neighbourhood6")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country5", "City6", "Neighbourhood7")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country4", "City5", "Neighbourhood8")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country6", "City7", "Neighbourhood9")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country4", "City5", "Neighbourhood10")),
			],
			[AlbumEntityFakes.WithReverseGeocodeAndId(1, new AlbumReverseGeocode(Address2: "City5"))],
			1,
			[
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country4", "City5", "Neighbourhood6")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country4", "City5", "Neighbourhood8")),
				PhotoEntityFakes.WithReverseGeocodes(ReverseGeocodeFakes.Exact("Country4", "City5", "Neighbourhood10")),
			]
		},
	};

	public static TheoryData<PhotoEntity[], AlbumEntity[], int, PhotoEntity[]> AlbumWithMixedConfigurationWithMatchingResult = new()
	{
		{
			[
				PhotoEntityFakes.WithId(1),
				PhotoEntityFakes.WithId(2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2000), 3),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2001), 4),
				PhotoEntityFakes.WithReverseGeocodesAndId(ReverseGeocodeFakes.Exact("Country1", "City1", "Neighbourhood1"), 5),
				PhotoEntityFakes.WithReverseGeocodesAndId(ReverseGeocodeFakes.Exact("Country2", "City2", "Neighbourhood2"), 6),
			],
			[
				AlbumEntityFakes.WithConfigurationId(1,
					new AlbumConfiguration(
						[1],
						new AlbumDateRange(DateTimeFakes.WithYear(2000), DateTimeFakes.WithYear(2000)),
						new AlbumReverseGeocode(Address1: "Country1")
					)
				)
			],
			1,
			[
				PhotoEntityFakes.WithId(1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2000),3),
				PhotoEntityFakes.WithReverseGeocodesAndId(ReverseGeocodeFakes.Exact("Country1", "City1", "Neighbourhood1"), 5),
			]
		},
	};

	[Theory]
	[MemberData(nameof(AlbumWithPhotoIdsConfigurationWithMatchingResult))]
	[MemberData(nameof(AlbumWithDateRangeConfigurationWithMatchingResult))]
	[MemberData(nameof(AlbumWithReserveGeocodeConfigurationWithMatchingResult))]
	[MemberData(nameof(AlbumWithMixedConfigurationWithMatchingResult))]
	public async Task GetAlbumPhotosById_GivenExistingAlbumId_ShouldMatchWithAlbumPhotoResult(PhotoEntity[] existingPhotos, AlbumEntity[] existingAlbums, int albumId,
		PhotoEntity[] expectedPhotosResult)
	{
		var sut = await DbServiceSetupWithPhotosAndAlbums(existingPhotos, existingAlbums);
		var (actualAlbumPhotoResultStatus, actualPhotosResult) = await sut.GetAlbumPhotosById(albumId);

		using (new AssertionScope())
		{
			actualAlbumPhotoResultStatus.Should().Be(AlbumPhotoResultStatus.Successful);

			actualPhotosResult.Should().BeEquivalentTo(expectedPhotosResult, c => c
				.Excluding(e => e.Id)
			);
		}
	}

	#endregion

	#region Breaking Flows

	public static TheoryData<AlbumEntity[], int> NonExistingAlbumIdTestData = new()
	{
		{
			[],
			1
		},
		{
			[AlbumEntityFakes.Sample(1)],
			2
		},
	};

	[Theory]
	[MemberData(nameof(NonExistingAlbumIdTestData))]
	public async Task GetAlbumPhotosById_GivenNotExistingAlbumId_ShouldReturnResultOfAlbumNotFound(AlbumEntity[] existingAlbums, int albumId)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);
		var actualAlbumPhotoResult = await sut.GetAlbumPhotosById(albumId);
		actualAlbumPhotoResult.Should().BeEquivalentTo(new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []));
	}

	public static TheoryData<AlbumEntity[], int> MisconfiguredAlbumIdTestData = new()
	{
		{
			[AlbumEntityFakes.WithRawConfigurationAndId("{", 1)],
			1
		},
	};

	[Theory]
	[MemberData(nameof(MisconfiguredAlbumIdTestData))]
	public async Task GetAlbumPhotosById_GivenMisconfiguredAlbumId_ShouldReturnResultOfExistingConfigurationNotInCorrectFormat(AlbumEntity[] existingAlbums, int albumId)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);
		var actualAlbumPhotoResult = await sut.GetAlbumPhotosById(albumId);
		actualAlbumPhotoResult.Should().BeEquivalentTo(new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, []));
	}

	#endregion

	#endregion

	#region GetAlbumPhotosByName

	#region Expected Flows

	public static TheoryData<PhotoEntity[], AlbumEntity[], string, PhotoEntity[]> AlbumWithPhotoIdsConfigurationWithMatchingResultByName = new()
	{
		{
			[
				PhotoEntityFakes.WithId(1),
			],
			[AlbumEntityFakes.WithPhotoIdsAndName("Album A", [1])],
			"Album A",
			[
				PhotoEntityFakes.WithId(1),
			]
		},
		{
			[
				PhotoEntityFakes.WithId(2),
				PhotoEntityFakes.WithId(3),
				PhotoEntityFakes.WithId(4),
				PhotoEntityFakes.WithId(5),
			],
			[AlbumEntityFakes.WithPhotoIdsAndName("Album B", [3, 4])],
			"Album B",
			[
				PhotoEntityFakes.WithId(3),
				PhotoEntityFakes.WithId(4),
			]
		},
	};

	[Theory]
	[MemberData(nameof(AlbumWithPhotoIdsConfigurationWithMatchingResultByName))]
	public async Task GetAlbumPhotosByName_GivenExistingAlbumName_ShouldMatchWithAlbumPhotoResult(PhotoEntity[] existingPhotos, AlbumEntity[] existingAlbums, string albumName,
		PhotoEntity[] expectedPhotosResult)
	{
		var sut = await DbServiceSetupWithPhotosAndAlbums(existingPhotos, existingAlbums);
		var (actualAlbumPhotoResultStatus, actualPhotosResult) = await sut.GetAlbumPhotosByName(albumName);

		using (new AssertionScope())
		{
			actualAlbumPhotoResultStatus.Should().Be(AlbumPhotoResultStatus.Successful);

			actualPhotosResult.Should().BeEquivalentTo(expectedPhotosResult, c => c
				.Excluding(e => e.Id)
			);
		}
	}

	#endregion

	#region Breaking Flows

	public static TheoryData<AlbumEntity[], string> NonExistingAlbumNameTestData = new()
	{
		{
			[],
			"Ghost Album"
		},
		{
			[AlbumEntityFakes.WithPhotoIdsAndName("Existing Album", [1])],
			"Other Album"
		},
	};

	[Theory]
	[MemberData(nameof(NonExistingAlbumNameTestData))]
	public async Task GetAlbumPhotosByName_GivenNotExistingAlbumName_ShouldReturnResultOfAlbumNotFound(AlbumEntity[] existingAlbums, string albumName)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);
		var actualAlbumPhotoResult = await sut.GetAlbumPhotosByName(albumName);
		actualAlbumPhotoResult.Should().BeEquivalentTo(new AlbumPhotoResult(AlbumPhotoResultStatus.AlbumNotFound, []));
	}

	public static TheoryData<AlbumEntity[], string> MisconfiguredAlbumNameTestData = new()
	{
		{
			[AlbumEntityFakes.WithRawConfiguration("{")],
			AlbumNameFakes.Valid()
		},
	};

	[Theory]
	[MemberData(nameof(MisconfiguredAlbumNameTestData))]
	public async Task GetAlbumPhotosByName_GivenMisconfiguredAlbumName_ShouldReturnResultOfExistingConfigurationNotInCorrectFormat(AlbumEntity[] existingAlbums, string albumName)
	{
		var sut = await DbServiceSetupWithAlbums(existingAlbums);
		var actualAlbumPhotoResult = await sut.GetAlbumPhotosByName(albumName);
		actualAlbumPhotoResult.Should().BeEquivalentTo(new AlbumPhotoResult(AlbumPhotoResultStatus.ExistingConfigurationNotInCorrectFormat, []));
	}

	#endregion

	#endregion

	#region GetPhotosByDate

	public static TheoryData<PhotoEntity[], int?, byte?, byte?, PhotoEntity[]> PhotosWithYearMatching = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2000), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2001), 2),
			],
			2000, null, null,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2000), 1),
			]
		},
	};

	public static TheoryData<PhotoEntity[], int?, byte?, byte?, PhotoEntity[]> PhotosWithMonthMatching = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(7), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(9), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(7), 3),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(11), 4),
			],
			null, 7, null,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(7), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(7), 3),
			]
		},
	};

	public static TheoryData<PhotoEntity[], int?, byte?, byte?, PhotoEntity[]> PhotosWithDayMatching = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 12), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 3, 17), 3),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 18), 4),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17), 5),
			],
			2020, 3, 17,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17), 5),
			]
		},
	};

	public static TheoryData<PhotoEntity[], int?, byte?, byte?, PhotoEntity[]> PhotosWithDeleted = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2017, 8, 19), 1, false),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2017, 8, 19), 2, true),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2024, 8, 19), 3, false),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2017, 8, 19), 4, false),

			],
			2017, 8, 19,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2017, 8, 19), 1, false),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2017, 8, 19), 4, false),
			]
		},
	};

	[Theory]
	[MemberData(nameof(PhotosWithYearMatching))]
	[MemberData(nameof(PhotosWithMonthMatching))]
	[MemberData(nameof(PhotosWithDayMatching))]
	[MemberData(nameof(PhotosWithDeleted))]
	public async Task GetPhotosByDate_GivenDateParameters_ShouldReturnMatchingPhotos(PhotoEntity[] existingPhotos, int? yearRequested, byte? monthRequested, byte? dayRequested,
		PhotoEntity[] expectedPhotosResult)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualPhotosResult = await sut.GetPhotosByDate(yearRequested, monthRequested, dayRequested);

		actualPhotosResult.Should().BeEquivalentTo(expectedPhotosResult);
	}

	#endregion

	#region GetPhotosByDateRange

	public static TheoryData<PhotoEntity[], DateTime, DateTime, PhotoEntity[]> PhotosWithDateRangeMatching = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17, 10, 0, 0), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 18, 10, 0, 0), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 19, 10, 0, 0), 3),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 20, 10, 0, 0), 4),
			],
			new DateTime(2020, 3, 17), new DateTime(2020, 3, 19),
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 17, 10, 0, 0), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 18, 10, 0, 0), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 3, 19, 10, 0, 0), 3),
			]
		},
	};

	public static TheoryData<PhotoEntity[], DateTime, DateTime, PhotoEntity[]> PhotosWithDateRangeExcludesDeleted = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 3, 17, 10, 0, 0), 1, false),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 3, 17, 10, 0, 0), 2, true),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 3, 18, 10, 0, 0), 3, false),
			],
			new DateTime(2020, 3, 17), new DateTime(2020, 3, 18),
			[
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 3, 17, 10, 0, 0), 1, false),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 3, 18, 10, 0, 0), 3, false),
			]
		},
	};

	[Theory]
	[MemberData(nameof(PhotosWithDateRangeMatching))]
	[MemberData(nameof(PhotosWithDateRangeExcludesDeleted))]
	public async Task GetPhotosByDateRange_GivenDateRangeParameters_ShouldReturnMatchingPhotos(PhotoEntity[] existingPhotos, DateTime start, DateTime end,
		PhotoEntity[] expectedPhotosResult)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualPhotosResult = await sut.GetPhotosByDateRange(start, end);

		actualPhotosResult.Should().BeEquivalentTo(expectedPhotosResult);
	}

	#endregion

	#region TotalPhotoCount

	public static TheoryData<PhotoEntity[], long> TotalPhotoCountWithVariousDbStates = new()
	{
		{
			[],
			0
		},
		{
			[
				PhotoEntityFakes.Sample(1)
			],
			1
		},
		{
			[
				PhotoEntityFakes.Sample(2),
				PhotoEntityFakes.Deleted(3),
				PhotoEntityFakes.Sample(4)
			],
			2
		},
	};

	[Theory]
	[MemberData(nameof(TotalPhotoCountWithVariousDbStates))]
	public async Task TotalPhotoCount_GivenExistingAlbumsStateInDbContext_ShouldReturnCorrectCount(PhotoEntity[] existingPhotos, long expectedPhotoCount)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualPhotoCount = await sut.TotalPhotoCount();
		actualPhotoCount.Should().Be(expectedPhotoCount);
	}

	#endregion

	#region TotalReverseGeocodeCacheCount

	public static TheoryData<ReverseGeocodeCacheEntity[], long> TotalReverseGeocodeCacheCountWithVariousDbStates = new()
	{
		{
			[],
			0
		},
		{
			[
				ReverseGeocodeCacheEntityFakes.Sample(1)
			],
			1
		},
		{
			[
				ReverseGeocodeCacheEntityFakes.Sample(2),
				ReverseGeocodeCacheEntityFakes.Sample(3)
			],
			2
		},
	};

	[Theory]
	[MemberData(nameof(TotalReverseGeocodeCacheCountWithVariousDbStates))]
	public async Task TotalReverseGeocodeCacheCount_GivenExistingAlbumsStateInDbContext_ShouldReturnCorrectCount(ReverseGeocodeCacheEntity[] existingPhotos, long expectedReverseGeocodeCache)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualReverseGeocodeCache = await sut.TotalReverseGeocodeCacheCount();
		actualReverseGeocodeCache.Should().Be(expectedReverseGeocodeCache);
	}

	#endregion

	#region Helpers

	private static async Task<DbService> DbServiceSetupWithPhotosAndAlbums(PhotoEntity[] existingPhotos, AlbumEntity[] existingAlbums)
	{
		var (dbService, archiveDbContextProvider) = DbServiceSetup();
		var archiveDbContext = archiveDbContextProvider.CreateOrGetInstance();
		await archiveDbContext.Photos.AddRangeAsync(existingPhotos);
		await archiveDbContext.Albums.AddRangeAsync(existingAlbums);
		await archiveDbContext.SaveChangesAsync();
		return dbService;
	}

	private static async Task<DbService> DbServiceSetupWithPhotos(PhotoEntity[] existingPhotos)
	{
		var (dbService, archiveDbContextProvider) = DbServiceSetup();
		var archiveDbContext = archiveDbContextProvider.CreateOrGetInstance();
		await archiveDbContext.Photos.AddRangeAsync(existingPhotos);
		await archiveDbContext.SaveChangesAsync();
		return dbService;
	}

	private static async Task<DbService> DbServiceSetupWithPhotos(ReverseGeocodeCacheEntity[] existingReverseGeocodeCacheEntities)
	{
		var (dbService, archiveDbContextProvider) = DbServiceSetup();
		var archiveDbContext = archiveDbContextProvider.CreateOrGetInstance();
		await archiveDbContext.ReverseGeocodeCache.AddRangeAsync(existingReverseGeocodeCacheEntities);
		await archiveDbContext.SaveChangesAsync();
		return dbService;
	}

	#endregion
}
