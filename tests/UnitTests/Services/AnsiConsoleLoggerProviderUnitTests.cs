using PhotoCli.Utils.Logging;
using Spectre.Console.Testing;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests.Services;

public class AnsiConsoleLoggerProviderUnitTests
{
	#region Constructor Tests

	[Fact]
	public void Constructor_WithNullLogLevel_ShouldSetDefaultLogLevelToWarning()
	{
		var toolOptions = ToolOptionsFakes.Valid();
		toolOptions.LogLevel = null;
		var consoleWriter = ConsoleWriterFakes.Valid();
		var ansiConsoleExtended = CreateAnsiConsoleExtended();

		var provider = new AnsiConsoleLoggerProvider(toolOptions, consoleWriter, ansiConsoleExtended);
		var logger = provider.CreateLogger("Test.Category");

		logger.Should().NotBeNull();
		logger.IsEnabled(LogLevel.Warning).Should().BeTrue();
		logger.IsEnabled(LogLevel.Information).Should().BeFalse();
	}

	[Fact]
	public void Constructor_WithDefaultLogLevel_ShouldSetCorrectDefaultLogLevel()
	{
		var toolOptions = ToolOptionsFakes.Valid();
		toolOptions.LogLevel = new Dictionary<string, string>
		{
			["Default"] = "Information"
		};
		var consoleWriter = ConsoleWriterFakes.Valid();
		var ansiConsoleExtended = CreateAnsiConsoleExtended();

		var provider = new AnsiConsoleLoggerProvider(toolOptions, consoleWriter, ansiConsoleExtended);
		var logger = provider.CreateLogger("Test.Category");

		logger.Should().NotBeNull();
		logger.IsEnabled(LogLevel.Information).Should().BeTrue();
		logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
	}

	[Fact]
	public void Constructor_WithValidNamespaceLogLevels_ShouldConfigureLogLevels()
	{
		var toolOptions = ToolOptionsFakes.Valid();
		toolOptions.LogLevel = new Dictionary<string, string>
		{
			["Default"] = "Warning",
			["PhotoCli.Services"] = "Debug",
			["PhotoCli.Runners"] = "Error"
		};
		var consoleWriter = ConsoleWriterFakes.Valid();
		var ansiConsoleExtended = CreateAnsiConsoleExtended();

		var provider = new AnsiConsoleLoggerProvider(toolOptions, consoleWriter, ansiConsoleExtended);
		var servicesLogger = provider.CreateLogger("PhotoCli.Services.TestService");
		var runnersLogger = provider.CreateLogger("PhotoCli.Runners.TestRunner");
		var otherLogger = provider.CreateLogger("PhotoCli.Other.TestClass");

		servicesLogger.IsEnabled(LogLevel.Debug).Should().BeTrue();
		runnersLogger.IsEnabled(LogLevel.Error).Should().BeTrue();
		runnersLogger.IsEnabled(LogLevel.Warning).Should().BeFalse();
		otherLogger.IsEnabled(LogLevel.Warning).Should().BeTrue();
		otherLogger.IsEnabled(LogLevel.Information).Should().BeFalse();
	}

	[Fact]
	public void Constructor_WithInvalidLogLevel_ShouldWriteErrorAndSkipConfiguration()
	{
		var testConsole = new TestConsole();
		var ansiConsoleExtended = new AnsiConsoleExtended(testConsole);
		var consoleWriter = new ConsoleWriter(ansiConsoleExtended);
		var toolOptions = ToolOptionsFakes.Valid();
		toolOptions.LogLevel = new Dictionary<string, string>
		{
			["Default"] = "Warning",
			["PhotoCli.Services"] = "InvalidLogLevel",
			["PhotoCli.Runners"] = "Debug"
		};

		var provider = new AnsiConsoleLoggerProvider(toolOptions, consoleWriter, ansiConsoleExtended);
		var servicesLogger = provider.CreateLogger("PhotoCli.Services.TestService");
		var runnersLogger = provider.CreateLogger("PhotoCli.Runners.TestRunner");

		var output = string.Join("", testConsole.Lines);
		output.Should().Contain("Invalid log level value of InvalidLogLevel for namespace PhotoCli.Services");

		servicesLogger.IsEnabled(LogLevel.Warning).Should().BeTrue();
		servicesLogger.IsEnabled(LogLevel.Information).Should().BeFalse();
		runnersLogger.IsEnabled(LogLevel.Debug).Should().BeTrue();
	}

	#endregion

	#region CreateLogger Tests

	[Fact]
	public void CreateLogger_WithCategoryName_ShouldReturnAnsiConsoleLogger()
	{
		var provider = CreateProvider();
		var logger = provider.CreateLogger("Test.Category");

		logger.Should().BeOfType<AnsiConsoleLogger>();
	}

	[Fact]
	public void CreateLogger_WithDifferentCategoryNames_ShouldReturnDifferentLoggers()
	{
		var provider = CreateProvider();
		var logger1 = provider.CreateLogger("Test.Category1");
		var logger2 = provider.CreateLogger("Test.Category2");

		logger1.Should().NotBeSameAs(logger2);
	}

	[Fact]
	public void CreateLogger_WithSameCategoryName_ShouldReturnDifferentInstances()
	{
		var provider = CreateProvider();
		var logger1 = provider.CreateLogger("Test.Category");
		var logger2 = provider.CreateLogger("Test.Category");

		logger1.Should().NotBeSameAs(logger2);
	}

	#endregion

	#region Helpers

	private static AnsiConsoleLoggerProvider CreateProvider()
	{
		var toolOptions = ToolOptionsFakes.Valid();
		toolOptions.LogLevel = new Dictionary<string, string> { ["Default"] = "Information" };
		return new AnsiConsoleLoggerProvider(toolOptions, ConsoleWriterFakes.Valid(), CreateAnsiConsoleExtended());
	}

	private static AnsiConsoleExtended CreateAnsiConsoleExtended()
	{
		return new AnsiConsoleExtended(new TestConsole());
	}

	#endregion
}
