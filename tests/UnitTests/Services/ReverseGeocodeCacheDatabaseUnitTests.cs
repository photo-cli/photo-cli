namespace PhotoCli.Tests.UnitTests.Services;

public class ReverseGeocodeCacheDatabaseUnitTests
{
	[Fact]
	public async Task TryGet_CacheMiss_ShouldReturnFalseWithDefaultValue()
	{
		var mockDbService = new Mock<IDbService>(MockBehavior.Strict);

		mockDbService.Setup(s => s
			.GetReverseGeocodeCache<CacheEntryFake>(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Valid()), It.IsAny<ReverseGeocodeProvider>()))
			.ReturnsAsync((CacheEntryFake?)null);

		var sut = new ReverseGeocodeCacheDatabase<CacheEntryFake>(mockDbService.Object, StatisticsFakes.Empty(), NullLogger<ReverseGeocodeCacheDatabase<CacheEntryFake>>.Instance);
		var result = await sut.TryGet(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Valid()), ReverseGeocodeProviderFakes.Valid());

		result.CacheHit.Should().BeFalse();
		result.Result.Should().BeNull();
		mockDbService.Verify(v => v.GetReverseGeocodeCache<CacheEntryFake>(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Valid()), It.IsAny<ReverseGeocodeProvider>()), Times.Once);
		mockDbService.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task TryGet_FirstTimeRequestNotFoundOnMemoryCacheButCacheHitFromDb_ShouldSReturnTrueWithValue()
	{
		var mockDbService = new Mock<IDbService>(MockBehavior.Strict);

		mockDbService.Setup(s => s
			.GetReverseGeocodeCache<CacheEntryFake>(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), It.IsAny<ReverseGeocodeProvider>()))
			.ReturnsAsync(CacheEntryFake.Sample(1));

		var sut = new ReverseGeocodeCacheDatabase<CacheEntryFake>(mockDbService.Object, StatisticsFakes.Empty(), NullLogger<ReverseGeocodeCacheDatabase<CacheEntryFake>>.Instance);

		var result = await sut.TryGet(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), ReverseGeocodeProviderFakes.Valid());

		result.CacheHit.Should().BeTrue();
		result.Result!.SampleId.Should().Be(1);
		mockDbService.Verify(v => v.GetReverseGeocodeCache<CacheEntryFake>(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), It.IsAny<ReverseGeocodeProvider>()), Times.Once);
		mockDbService.VerifyNoOtherCalls();
	}

	[Fact]
	public async Task TryGet_CacheHitRequestsAfterFirstTimeDirectlyFromMemoryCacheNotFromDb_ShouldReturnTrueWithValue()
	{
		var mockDbService = new Mock<IDbService>(MockBehavior.Strict);

		mockDbService.Setup(s => s
			.GetReverseGeocodeCache<CacheEntryFake>(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), It.IsAny<ReverseGeocodeProvider>()))
			.ReturnsAsync(CacheEntryFake.Sample(1));

		var sut = new ReverseGeocodeCacheDatabase<CacheEntryFake>(mockDbService.Object, StatisticsFakes.Empty(), NullLogger<ReverseGeocodeCacheDatabase<CacheEntryFake>>.Instance);

		await sut.TryGet(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), ReverseGeocodeProviderFakes.Valid());
		var result = await sut.TryGet(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), ReverseGeocodeProviderFakes.Valid());

		result.CacheHit.Should().BeTrue();
		result.Result!.SampleId.Should().Be(1);
		mockDbService.Verify(v => v.GetReverseGeocodeCache<CacheEntryFake>(ReverseGeocodeRequestFakes.WithCoordinate(CoordinateFakes.Sample(1)), It.IsAny<ReverseGeocodeProvider>()), Times.Once);
		mockDbService.VerifyNoOtherCalls();
	}
}
