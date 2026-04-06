namespace PhotoCli.Tests.IntegrationTests.DbContext;

public class McpDbServiceIntegrationTests : DbServiceIntegrationTestsBase
{
	#region SearchPhotos

	#region Expected Flows

	public static TheoryData<PhotoEntity[], DateTime?, DateTime?, int, PhotoEntity[]> SearchPhotosWithDateFilters = new()
	{
		// Start date filter - returns photos on or after start date, ordered by date desc
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 1, 1), 3),
			],
			new DateTime(2020, 6, 1), null, 100,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 1, 1), 3),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
			]
		},
		// End date filter - returns photos on or before end date, ordered by date desc
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 1, 1), 3),
			],
			null, new DateTime(2020, 6, 1), 100,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
			]
		},
		// Date range filter
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 1, 1), 3),
			],
			new DateTime(2020, 3, 1), new DateTime(2020, 8, 1), 100,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
			]
		},
		// Limit applied - returns most recent photos up to limit
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 1, 1), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 1, 1), 3),
			],
			null, null, 2,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2021, 1, 1), 3),
				PhotoEntityFakes.WithPhotoTakenDateAndId(new DateTime(2020, 6, 1), 2),
			]
		},
		// Deleted photos excluded
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 1, 1), 1, false),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 6, 1), 2, true),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2021, 1, 1), 3, false),
			],
			null, null, 100,
			[
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2021, 1, 1), 3, false),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(new DateTime(2020, 1, 1), 1, false),
			]
		},
	};

	[Theory]
	[MemberData(nameof(SearchPhotosWithDateFilters))]
	public async Task SearchPhotos_GivenDateFilters_ShouldReturnMatchingPhotosOrderedByDateDesc(
		PhotoEntity[] existingPhotos, DateTime? start, DateTime? end, int limit, PhotoEntity[] expectedPhotos)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualPhotos = await sut.SearchPhotos(start, end, null, limit);
		actualPhotos.Should().BeEquivalentTo(expectedPhotos, c => c.WithStrictOrdering());
	}

	public static TheoryData<PhotoEntity[], string, PhotoEntity[]> SearchPhotosWithLocationFilter = new()
	{
		{
			[
				WithReverseGeocodeFormattedAndId("Paris, France", 1),
				WithReverseGeocodeFormattedAndId("London, UK", 2),
				WithReverseGeocodeFormattedAndId(null, 3),
			],
			"Paris",
			[
				WithReverseGeocodeFormattedAndId("Paris, France", 1),
			]
		},
		{
			[
				WithReverseGeocodeFormattedAndId("Paris, France", 1),
				WithReverseGeocodeFormattedAndId("London, UK", 2),
				WithReverseGeocodeFormattedAndId("New York, USA", 3),
			],
			"UK",
			[
				WithReverseGeocodeFormattedAndId("London, UK", 2),
			]
		},
	};

	[Theory]
	[MemberData(nameof(SearchPhotosWithLocationFilter))]
	public async Task SearchPhotos_GivenLocationFilter_ShouldReturnPhotosMatchingLocation(
		PhotoEntity[] existingPhotos, string location, PhotoEntity[] expectedPhotos)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualPhotos = await sut.SearchPhotos(null, null, location, 100);
		actualPhotos.Should().BeEquivalentTo(expectedPhotos);
	}

	#endregion

	#endregion

	#region GetPhotoByPath

	#region Expected Flows

	public static TheoryData<PhotoEntity[], string, PhotoEntity?> GetPhotoByPathTestData = new()
	{
		// Existing path returns matching photo
		{
			[PhotoEntityFakes.CreateWithExifData(path: "photos/vacation.jpg")],
			"photos/vacation.jpg",
			PhotoEntityFakes.CreateWithExifData(path: "photos/vacation.jpg")
		},
		// Non-existing path returns null
		{
			[PhotoEntityFakes.CreateWithExifData(path: "photos/vacation.jpg")],
			"photos/other.jpg",
			null
		},
		// Empty database returns null
		{
			[],
			"photos/any.jpg",
			null
		},
	};

	[Theory]
	[MemberData(nameof(GetPhotoByPathTestData))]
	public async Task GetPhotoByPath_GivenPath_ShouldReturnMatchingPhoto(
		PhotoEntity[] existingPhotos, string path, PhotoEntity? expectedPhoto)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualPhoto = await sut.GetPhotoByPath(path);
		actualPhoto.Should().BeEquivalentTo(expectedPhoto, c => c.Excluding(e => e!.Id));
	}

	#endregion

	#region Breaking Flows

	[Fact]
	public async Task GetPhotoByPath_DeletedPhoto_ShouldReturnNull()
	{
		var deletedPhoto = PhotoEntityFakes.CreateWithExifData(path: "photos/deleted.jpg") with { IsDeleted = true };
		var sut = await DbServiceSetupWithPhotos([deletedPhoto]);
		var actualPhoto = await sut.GetPhotoByPath("photos/deleted.jpg");
		actualPhoto.Should().BeNull();
	}

	#endregion

	#endregion

	#region GetPhotoStatistics

	#region Expected Flows

	public static TheoryData<PhotoEntity[], string, List<PhotoStatisticsRow>> GetPhotoStatisticsGroupByYear = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2020), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2020), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2021), 3),
			],
			"year",
			[new PhotoStatisticsRow(2020, null, null, 2), new PhotoStatisticsRow(2021, null, null, 1)]
		},
		// Deleted photos excluded from year grouping
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2020), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndIdAdIsDeleted(DateTimeFakes.WithYear(2020), 2, true),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2021), 3),
			],
			"year",
			[new PhotoStatisticsRow(2020, null, null, 1), new PhotoStatisticsRow(2021, null, null, 1)]
		},
	};

	public static TheoryData<PhotoEntity[], string, List<PhotoStatisticsRow>> GetPhotoStatisticsGroupByMonth = new()
	{
		{
			[
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(1), 1),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(1), 2),
				PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithMonth(3), 3),
			],
			"month",
			[
				new PhotoStatisticsRow(DateTimeFakes.WithMonth(1).Year, 1, null, 2),
				new PhotoStatisticsRow(DateTimeFakes.WithMonth(3).Year, 3, null, 1),
			]
		},
	};

	public static TheoryData<PhotoEntity[], string, List<PhotoStatisticsRow>> GetPhotoStatisticsGroupByLocation = new()
	{
		// "location" groupBy uses Address1, ordered by count desc
		{
			[
				CreateWithAddress1AndId("City1", 1),
				CreateWithAddress1AndId("City2", 2),
				CreateWithAddress1AndId("City2", 3),
				CreateWithAddress1AndId("City2", 4),
			],
			"location",
			[
				new PhotoStatisticsRow(null, null, "City2", 3),
				new PhotoStatisticsRow(null, null, "City1", 1),
			]
		},
		// "address1" is alias for "location"
		{
			[
				CreateWithAddress1AndId("Paris", 1),
				CreateWithAddress1AndId("Paris", 2),
				CreateWithAddress1AndId("London", 3),
			],
			"address1",
			[
				new PhotoStatisticsRow(null, null, "Paris", 2),
				new PhotoStatisticsRow(null, null, "London", 1),
			]
		},
		// Deleted photos excluded from location grouping
		{
			[
				CreateWithAddress1AndId("City1", 1),
				(CreateWithAddress1AndId("City1", 2)) with { IsDeleted = true },
				CreateWithAddress1AndId("City2", 3),
			],
			"location",
			[
				new PhotoStatisticsRow(null, null, "City1", 1),
				new PhotoStatisticsRow(null, null, "City2", 1),
			]
		},
	};

	[Theory]
	[MemberData(nameof(GetPhotoStatisticsGroupByYear))]
	[MemberData(nameof(GetPhotoStatisticsGroupByMonth))]
	public async Task GetPhotoStatistics_GivenGroupByYearOrMonth_ShouldReturnOrderedStatisticsRows(
		PhotoEntity[] existingPhotos, string groupBy, List<PhotoStatisticsRow> expectedRows)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualRows = await sut.GetPhotoStatistics(groupBy);
		actualRows.Should().BeEquivalentTo(expectedRows, c => c.WithStrictOrdering());
	}

	[Theory]
	[MemberData(nameof(GetPhotoStatisticsGroupByLocation))]
	public async Task GetPhotoStatistics_GivenLocationGroupBy_ShouldReturnPhotosGroupedByAddressOrderedByCountDesc(
		PhotoEntity[] existingPhotos, string groupBy, List<PhotoStatisticsRow> expectedRows)
	{
		var sut = await DbServiceSetupWithPhotos(existingPhotos);
		var actualRows = await sut.GetPhotoStatistics(groupBy);
		actualRows.Should().BeEquivalentTo(expectedRows, c => c.WithStrictOrdering());
	}

	#endregion

	#region Breaking Flows

	public static TheoryData<string> UnknownGroupByValues = new()
	{
		{ "unknown" },
		{ "invalid" },
		{ "week" },
	};

	[Theory]
	[MemberData(nameof(UnknownGroupByValues))]
	public async Task GetPhotoStatistics_GivenUnknownGroupBy_ShouldReturnEmptyList(string groupBy)
	{
		var sut = await DbServiceSetupWithPhotos([PhotoEntityFakes.WithPhotoTakenDateAndId(DateTimeFakes.WithYear(2020), 1)]);
		var actualRows = await sut.GetPhotoStatistics(groupBy);
		actualRows.Should().BeEmpty();
	}

	#endregion

	#endregion

	#region FindPhotosNearLocation

	#region Expected Flows

	[Fact]
	public async Task FindPhotosNearLocation_PhotosWithinRadius_ShouldBeReturned()
	{
		var nearPhoto = CreateWithCoordinateAndId(40.0, 30.0, 1);
		var farPhoto = CreateWithCoordinateAndId(0.0, 0.0, 2); // ~5000 km away
		var sut = await DbServiceSetupWithPhotos([nearPhoto, farPhoto]);

		var results = await sut.FindPhotosNearLocation(40.0, 30.0, 1000.0, 10);

		results.Should().ContainSingle().Which.Path.Should().Be("1.jpg");
	}

	[Fact]
	public async Task FindPhotosNearLocation_ResultsOrderedByDistanceAscending()
	{
		var closestPhoto = CreateWithCoordinateAndId(40.0, 30.0, 1);
		var midPhoto = CreateWithCoordinateAndId(40.5, 30.0, 2);
		var farthestPhoto = CreateWithCoordinateAndId(41.0, 30.0, 3);
		var sut = await DbServiceSetupWithPhotos([farthestPhoto, midPhoto, closestPhoto]);

		var results = await sut.FindPhotosNearLocation(40.0, 30.0, 500.0, 10);

		results.Should().HaveCount(3);
		results.Select(r => r.Path).Should().Equal("1.jpg", "2.jpg", "3.jpg");
		results.Select(r => r.DistanceKm).Should().BeInAscendingOrder();
	}

	[Fact]
	public async Task FindPhotosNearLocation_WithLimit_ShouldReturnClosestPhotosUpToLimit()
	{
		var closestPhoto = CreateWithCoordinateAndId(40.0, 30.0, 1);
		var midPhoto = CreateWithCoordinateAndId(40.5, 30.0, 2);
		var farthestPhoto = CreateWithCoordinateAndId(41.0, 30.0, 3);
		var sut = await DbServiceSetupWithPhotos([closestPhoto, midPhoto, farthestPhoto]);

		var results = await sut.FindPhotosNearLocation(40.0, 30.0, 500.0, 2);

		results.Should().HaveCount(2);
		results.Select(r => r.Path).Should().Equal("1.jpg", "2.jpg");
	}

	#endregion

	#region Breaking Flows

	[Fact]
	public async Task FindPhotosNearLocation_DeletedPhotos_ShouldBeExcluded()
	{
		var activePhoto = CreateWithCoordinateAndId(40.0, 30.0, 1);
		var deletedPhoto = CreateWithCoordinateAndId(40.0, 30.0, 2) with { IsDeleted = true };
		var sut = await DbServiceSetupWithPhotos([activePhoto, deletedPhoto]);

		var results = await sut.FindPhotosNearLocation(40.0, 30.0, 100.0, 10);

		results.Should().ContainSingle().Which.Path.Should().Be("1.jpg");
	}

	[Fact]
	public async Task FindPhotosNearLocation_PhotosWithoutCoordinates_ShouldBeExcluded()
	{
		var photoWithCoordinate = CreateWithCoordinateAndId(40.0, 30.0, 1);
		var photoWithoutCoordinate = PhotoEntityFakes.WithId(2);
		var sut = await DbServiceSetupWithPhotos([photoWithCoordinate, photoWithoutCoordinate]);

		var results = await sut.FindPhotosNearLocation(40.0, 30.0, 1000.0, 10);

		results.Should().ContainSingle().Which.Path.Should().Be("1.jpg");
	}

	#endregion

	#endregion

	#region Helpers

	private static PhotoEntity WithReverseGeocodeFormattedAndId(string? reverseGeocodeFormatted, long id)
	{
		return PhotoEntityFakes.CreateWithExifData(id: id) with { ReverseGeocodeFormatted = reverseGeocodeFormatted };
	}

	private static PhotoEntity CreateWithAddress1AndId(string address1, long id)
	{
		return PhotoEntityFakes.CreateWithExifData(exifData: ExifDataFakes.WithReverseGeocodes([address1]), id: id);
	}

	private static PhotoEntity CreateWithCoordinateAndId(double latitude, double longitude, long id)
	{
		return PhotoEntityFakes.CreateWithExifData(exifData: ExifDataFakes.WithCoordinate(new Coordinate(latitude, longitude)), id: id);
	}

	private static async Task<DbService> DbServiceSetupWithPhotos(PhotoEntity[] existingPhotos)
	{
		var (dbService, archiveDbContextProvider) = DbServiceSetup();
		var archiveDbContext = archiveDbContextProvider.CreateOrGetInstance();
		await archiveDbContext.Photos.AddRangeAsync(existingPhotos);
		await archiveDbContext.SaveChangesAsync();
		return dbService;
	}

	#endregion
}
