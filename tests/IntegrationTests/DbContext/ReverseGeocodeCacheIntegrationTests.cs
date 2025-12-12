using Microsoft.EntityFrameworkCore;

namespace PhotoCli.Tests.IntegrationTests.DbContext;

public class ReverseGeocodeCacheIntegrationTests : DbServiceIntegrationTestsBase
{
	public static TheoryData<ReverseGeocodeRequest> ReverseGeocodeRequests = new()
	{
		ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.SampleRounded(1)),
		ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.SampleRounded(2)),
		ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.SampleRounded(3)),
	};

	[Theory]
	[MemberData(nameof(ReverseGeocodeRequests))]
	public async Task SaveReverseGeocodeCache_GivenReverseGeocodeResponseAndCoordinate_ShouldMatchOnDb(ReverseGeocodeRequest request)
	{
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var provider = ReverseGeocodeProviderFakes.Valid();
		var response = ReverseGeocodeResponseFakes.WithCoordinate(request.Coordinate);

		await sut.SaveReverseGeocodeCache(request, response, provider);

		var reverseGeocodeResponseBytes = JsonSerializer.SerializeToUtf8Bytes(response);
		var reverseGeocodeCacheOnDb = await archiveDbContextProvider.CreateOrGetInstance().ReverseGeocodeCache.SingleAsync();
		using (new AssertionScope())
		{
			reverseGeocodeCacheOnDb.Id.Should().BePositive();
			reverseGeocodeCacheOnDb.Latitude.Should().Be(request.Coordinate.Latitude);
			reverseGeocodeCacheOnDb.Longitude.Should().Be(request.Coordinate.Longitude);
			reverseGeocodeCacheOnDb.CreatedAt.Should().BeAfter(DateTime.Today);
			reverseGeocodeCacheOnDb.Precision.Should().Be(ToolOptionFakes.CoordinatePrecisionDefault);
			reverseGeocodeCacheOnDb.Provider.Should().Be(provider);
			reverseGeocodeCacheOnDb.Response.Should().Equal(reverseGeocodeResponseBytes);
		}
	}

	[Fact]
	public async Task GetReverseGeocodeCache_GivenGoogleMapsReverseGeocodeRequest_ShouldReturnDeserializedMatchingResponseOnDb()
	{
		await GetReverseGeocodeCacheShouldReturnDeserializedMatchingResponseOnDb(ReverseGeocodeProvider.GoogleMaps, GoogleMapsFullResponseFakes.Valid);
	}

	[Fact]
	public async Task GetReverseGeocodeCache_GivenBigDataCloudReverseGeocodeRequest_ShouldReturnDeserializedMatchingResponseOnDb()
	{
		await GetReverseGeocodeCacheShouldReturnDeserializedMatchingResponseOnDb(ReverseGeocodeProvider.BigDataCloud, BigDataCloudFullResponseFakes.Valid);
	}

	[Fact]
	public async Task GetReverseGeocodeCache_GivenOpenStreetMapFoundationGeocodeRequest_ShouldReturnDeserializedMatchingResponseOnDb()
	{
		await GetReverseGeocodeCacheShouldReturnDeserializedMatchingResponseOnDb(ReverseGeocodeProvider.OpenStreetMapFoundation, OpenStreetMapFullResponseFakes.Valid);
	}

	private async Task GetReverseGeocodeCacheShouldReturnDeserializedMatchingResponseOnDb<T>(ReverseGeocodeProvider reverseGeocodeProvider, Func<Coordinate, T> coordinateResponseSelector)
	{
		List<Coordinate> coordinates = [CoordinateFakes.SampleRounded(1), CoordinateFakes.SampleRounded(2), CoordinateFakes.SampleRounded(3)];
		var (sut, archiveDbContextProvider) = DbServiceSetup();
		var archiveDbContext = archiveDbContextProvider.CreateOrGetInstance();

		// Setup database cache for these 3 coordinates to check SQL query is filtering correctly
		foreach (var coordinateToAdd in coordinates)
		{
			var reverseGeocodeResponseBytesForCoordinate = JsonSerializer.SerializeToUtf8Bytes(coordinateResponseSelector(coordinateToAdd));

			var reverseGeocodeCacheEntity = new ReverseGeocodeCacheEntity(reverseGeocodeProvider, coordinateToAdd.Latitude, coordinateToAdd.Longitude,
				reverseGeocodeResponseBytesForCoordinate, ToolOptionFakes.CoordinatePrecisionDefault, null, DateTime.Today);

			await archiveDbContext.ReverseGeocodeCache.AddAsync(reverseGeocodeCacheEntity);
		}
		await archiveDbContext.SaveChangesAsync();

		using (new AssertionScope())
		{
			foreach (var coordinateToCheck in coordinates)
			{
				var reverseGeocodeRequest = new ReverseGeocodeRequest(coordinateToCheck);
				var actualCachedResponse = await sut.GetReverseGeocodeCache<T>(reverseGeocodeRequest, reverseGeocodeProvider);
				actualCachedResponse.Should().BeEquivalentTo(coordinateResponseSelector(coordinateToCheck));
			}
		}
	}
}
