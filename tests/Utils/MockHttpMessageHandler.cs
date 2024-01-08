using Moq.Protected;

namespace PhotoCli.Tests.Utils;

public static class MockHttpMessageHandler
{
	public static (Mock<HttpMessageHandler>, HttpClient) WithResponse(string responseRawBody)
	{
		var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

		httpMessageHandlerMock
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

		var httpClient = new HttpClient(httpMessageHandlerMock.Object)
		{
			BaseAddress = new Uri("http://valid-uri-needed"),
		};

		return (httpMessageHandlerMock, httpClient);
	}
}
