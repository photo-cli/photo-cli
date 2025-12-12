using Moq.Protected;

namespace PhotoCli.Tests.UnitTests.Services.ReverseGeocodes;

public class ReverseGeocodeServiceUnitTestBase<TResponse>
{
	private readonly ReverseGeocodeProvider _reverseGeocodeProvider;
	protected static string PhotoPathValid = MockFileSystemHelper.Path("/folder/photo.jpg");
	protected readonly PhotoFile PhotoFileValid = PhotoFileFakes.Create(PhotoPathValid);

	protected ReverseGeocodeServiceUnitTestBase(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		_reverseGeocodeProvider = reverseGeocodeProvider;
	}

	protected IReverseGeocodeCache<TResponse> ReverseGeocodeCacheMock(Coordinate coordinateCacheKey, TResponse responseCacheResult)
	{
		var request = new ReverseGeocodeRequest(coordinateCacheKey);
		var reverseGeocodeCacheMock = new Mock<IReverseGeocodeCache<TResponse>>();

		reverseGeocodeCacheMock
			.Setup(x => x.TryGet(It.IsAny<ReverseGeocodeRequest>(), _reverseGeocodeProvider))
			.ReturnsAsync((ReverseGeocodeRequest req, ReverseGeocodeProvider _) =>
				req.Equals(request)
					? new ReverseGeocodeCacheResult<TResponse>(true, responseCacheResult)
					: new ReverseGeocodeCacheResult<TResponse>(false, default));

		return reverseGeocodeCacheMock.Object;
	}

	protected IReverseGeocodeCache<TResponse> ReverseGeocodeCacheAlwaysMiss()
	{
		var reverseGeocodeCacheMock = new Mock<IReverseGeocodeCache<TResponse>>();

		reverseGeocodeCacheMock
			.Setup(x => x.TryGet(It.IsAny<ReverseGeocodeRequest>(), _reverseGeocodeProvider))
			.ReturnsAsync(() => new ReverseGeocodeCacheResult<TResponse>(false, default));

		return reverseGeocodeCacheMock.Object;
	}

	protected static void VerifyMockMessageHandlerSendRequestExactly(Times times, Mock<HttpMessageHandler> mockHttpMessageHandler)
	{
		mockHttpMessageHandler.Protected().Verify("SendAsync", times, ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
	}
}
