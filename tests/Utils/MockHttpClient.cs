using Moq.Protected;

namespace PhotoCli.Tests.Utils;

public static class MockHttpClient
{
	public static HttpClient WithResponse(string responseRawBody)
	{
		var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
		handlerMock
			.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(responseRawBody),
			});

		var httpClient = new HttpClient(handlerMock.Object)
		{
			BaseAddress = new Uri("http://valid-uri-needed"),
		};
		return httpClient;
	}

	public static HttpClient WithError()
	{
		var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
		handlerMock
			.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.ServiceUnavailable,
			});

		var httpClient = new HttpClient(handlerMock.Object)
		{
			BaseAddress = new Uri("http://valid-uri-needed"),
		};
		return httpClient;
	}
}
