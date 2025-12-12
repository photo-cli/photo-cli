using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.Utils;

public static class MockLoggerExtensions
{
	public static void VerifyAllLogStatementsAtLeastOnce<T>(this Mock<ILogger<T>> loggerMock, LogLevel logLevel, bool verifyNoOtherCalls, params string[] logStatements)
	{
		foreach (var logStatement in logStatements)
		{
			loggerMock.Verify(v => v.Log(logLevel, It.IsAny<EventId>(), It.Is<It.IsAnyType>((state, type) => state.ToString() == logStatement),
				null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
		}
		if (verifyNoOtherCalls)
			loggerMock.VerifyNoOtherCalls();
	}

	public static void VerifyExceptionLogStatement<T>(this Mock<ILogger<T>> loggerMock, LogLevel logLevel, Exception exception, string logStatement, bool verifyNoOtherCalls)
	{
		loggerMock.Verify(v => v.Log(logLevel, It.IsAny<EventId>(), It.Is<It.IsAnyType>((state, type) => state.ToString() == logStatement),
			exception, It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

		if (verifyNoOtherCalls)
			loggerMock.VerifyNoOtherCalls();
	}
}
