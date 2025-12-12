namespace PhotoCli.Tests.UnitTests.Services;

public class ReverseGeocodeCacheMemoryUnitTests
{
	[Fact]
	public async Task TryGet_CacheMissOnEmptyStore_ShouldReturnFalseWithDefaultValue()
	{
		var sut = ReverseGeocodeCacheMemory();
		var result = await sut.TryGet(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Valid()), ReverseGeocodeProviderFakes.Valid());

		result.CacheHit.Should().BeFalse();
		result.Result.Should().BeNull();
	}

	[Fact]
	public async Task TryGet_CacheMissOnNotExistEntry_ShouldReturnFalseWithDefaultValue()
	{
		var sut = ReverseGeocodeCacheMemory();
		await sut.SetResponse(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), ReverseGeocodeProviderFakes.Valid(), new CacheEntryFake(1));
		await sut.SetResponse(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(2)), ReverseGeocodeProviderFakes.Valid(), new CacheEntryFake(2));

		var result = await sut.TryGet(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(3)), ReverseGeocodeProviderFakes.Valid());

		result.CacheHit.Should().BeFalse();
		result.Result.Should().BeNull();
	}

	[Fact]
	public async Task TryGet_CacheHit_ShouldReturnTrueWithValue()
	{
		var sut = ReverseGeocodeCacheMemory();
		await sut.SetResponse(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), ReverseGeocodeProviderFakes.Valid(), new CacheEntryFake(1));
		await sut.SetResponse(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(2)), ReverseGeocodeProviderFakes.Valid(), new CacheEntryFake(2));

		var result = await sut.TryGet(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), ReverseGeocodeProviderFakes.Valid());

		result.CacheHit.Should().BeTrue();
		result.Result!.SampleId.Should().Be(1);
	}

	[Fact]
	public async Task SetResponse_ShouldComplete()
	{
		var sut = ReverseGeocodeCacheMemory();
		await sut.SetResponse(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), ReverseGeocodeProviderFakes.Valid(), new CacheEntryFake(1));
	}

	private static ReverseGeocodeCacheMemory<CacheEntryFake> ReverseGeocodeCacheMemory()
	{
		return new ReverseGeocodeCacheMemory<CacheEntryFake>(StatisticsFakes.Empty(), NullLogger<ReverseGeocodeCacheMemory<CacheEntryFake>>.Instance);
	}
}
