using System.Collections.Concurrent;
using PhotoCli.Utils.Logging;
using Spectre.Console.Testing;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

#pragma warning disable CA2254
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace PhotoCli.Tests.UnitTests.Services;

public class AnsiConsoleLoggerUnitTests
{
	#region IsEnabled

	[Theory]
	[InlineData(LogLevel.Information, LogLevel.Information, true)]
	[InlineData(LogLevel.Error, LogLevel.Information, true)]
	[InlineData(LogLevel.Information, LogLevel.Error, false)]
	[InlineData(LogLevel.Error, LogLevel.Critical, false)]
	public void IsEnabled_NoLogLevelsConfigured_ShouldUseDefaultLevelAndMatchResult(LogLevel testLevel, LogLevel defaultLevel, bool expectedResult)
	{
		var logger = CreateLogger(defaultLogLevel: defaultLevel, logLevels: null, logLevelCacheByNamespace: null);
		ValidateLoggerEnabledState(logger, testLevel, expectedResult);
	}

	public static TheoryData<string, LogLevel, bool, ConcurrentDictionary<string, LogLevel>> MatchingFullNamespaceSingleEntry = new()
	{
		{
			// Test level same with the configuration -> true
			NamespaceSample(1), LogLevel.Warning, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(1)] = LogLevel.Warning,
			}
		},
		{
			// Test level higher than the configuration -> true
			NamespaceSample(2), LogLevel.Error, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(2)] = LogLevel.Information,
			}
		},
		{
			// Test level lower than the configuration -> false
			NamespaceSample(3), LogLevel.Error, false,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(3)] = LogLevel.Critical,
			}
		},
	};

	public static TheoryData<string, LogLevel, bool, ConcurrentDictionary<string, LogLevel>> MatchingFullNamespaceMultipleEntries = new()
	{
		{
			// Test level same with the configuration -> true
			NamespaceSample(4), LogLevel.Warning, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(4)] = LogLevel.Warning,
				[NamespaceSample(5)] = LogLevel.Error,
			}
		},
		{
			// Test level higher than the configuration -> true
			NamespaceSample(6), LogLevel.Error, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(7)] = LogLevel.Error,
				[NamespaceSample(6)] = LogLevel.Information,
				[NamespaceSample(8)] = LogLevel.Trace,
			}
		},
		{
			// Test level lower than the configuration -> false
			NamespaceSample(9), LogLevel.Error, false,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(10)] = LogLevel.Debug,
				[NamespaceSample(9)] = LogLevel.Critical,
				[NamespaceSample(11)] = LogLevel.Warning,
			}
		},
	};

	public static TheoryData<string, LogLevel, bool, ConcurrentDictionary<string, LogLevel>> MatchingClosestNamespace = new()
	{
		{
			// Test level same with the configuration -> true
			"Root1.Sub1.Class1", LogLevel.Warning, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				["Root1.Sub1.Class2"] = LogLevel.Error, // 3rd part not matching
				["Root1.Sub1"] = LogLevel.Warning, // closest sub match
				["Root2.Sub1"] = LogLevel.Error, // 1st, 2nd part not matching
				["Root1.Sub2"] = LogLevel.Information, // 2nd part not matching
			}
		},
		{
			// Test level higher than the configuration -> true
			"Root3.Sub3.Class3", LogLevel.Error, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				["Root3.Sub4"] = LogLevel.Critical, // not sub match, the end is not matching
				["Root3"] = LogLevel.Information, // closest sub match
				["Root3.Sub3.Class4"] = LogLevel.Error, // 2nd part not matching
			}
		},
		{
			// Test level lower than the configuration -> false
			"Root5.Sub5.Class5", LogLevel.Error, false,
			new ConcurrentDictionary<string, LogLevel>
			{
				["Root5"] = LogLevel.Information, // 2nd closest match, not picking this
				["Root5.Sub6"] = LogLevel.Information, // 2nd part not matching
				["Root5.Sub5.Class7"] = LogLevel.Information, // 2nd part not matching
				["Root5.Sub5"] = LogLevel.Critical, // closest sub match
			}
		},
	};

	[Theory]
	[MemberData(nameof(MatchingFullNamespaceSingleEntry))]
	[MemberData(nameof(MatchingFullNamespaceMultipleEntries))]
	[MemberData(nameof(MatchingClosestNamespace))]
	public void IsEnabled_GivenNamespaceLogLevelsMatchingConfiguration_ShouldMatchResult(string loggerNamespaceCategoryName, LogLevel testLevel, bool expectedResult,
		ConcurrentDictionary<string, LogLevel> logLevels)
	{
		var logger = CreateLogger(loggerNamespaceCategoryName, logLevels: logLevels);
		ValidateLoggerEnabledState(logger, testLevel, expectedResult);
	}

	public static TheoryData<string, LogLevel, LogLevel, bool, ConcurrentDictionary<string, LogLevel>> MissingLogLevelConfigurationUsingDefaultLevel = new()
	{
		{
			// Test level same with the configuration -> true
			NamespaceSample(12), LogLevel.Warning, LogLevel.Warning, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(13)] = LogLevel.Debug,
			}
		},
		{
			// Test level higher than the configuration -> true
			NamespaceSample(14), LogLevel.Information, LogLevel.Error, true,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(15)] = LogLevel.Critical,
				[NamespaceSample(16)] = LogLevel.Information,
			}
		},
		{
			// Test level lower than the configuration -> false
			NamespaceSample(17), LogLevel.Information, LogLevel.Debug, false,
			new ConcurrentDictionary<string, LogLevel>
			{
				[NamespaceSample(18)] = LogLevel.Information,
				[NamespaceSample(19)] = LogLevel.Warning,
				[NamespaceSample(20)] = LogLevel.Critical,
			}
		},
	};

	[Theory]
	[MemberData(nameof(MissingLogLevelConfigurationUsingDefaultLevel))]
	public void IsEnabled_GivenNamespaceLogLevelsMissingConfiguration_ShouldUseDefaultLogLevel(string loggerNamespaceCategoryName, LogLevel defaultLogLevel, LogLevel testLevel, bool expectedResult,
		ConcurrentDictionary<string, LogLevel> logLevels)
	{
		var logger = CreateLogger(loggerNamespaceCategoryName, defaultLogLevel, logLevels: logLevels);
		ValidateLoggerEnabledState(logger, testLevel, expectedResult);
	}

	private static void ValidateLoggerEnabledState(AnsiConsoleLogger logger, LogLevel testLevel, bool expectedResult)
	{
		var result = logger.IsEnabled(testLevel);
		result.Should().Be(expectedResult);
	}

	[Fact]
	public void IsEnabled_FirstInvocation_ShouldAddToLogLevelCacheByNamespaceDictionary()
	{
		var categoryName = "RootNamespace1.TestClass1";
		var logLevel = LogLevel.Error;
		var logLevels = new ConcurrentDictionary<string, LogLevel>
		{
			[categoryName] = logLevel
		};
		var logLevelCacheByNamespace = new ConcurrentDictionary<string, LogLevel>();
		var logger = CreateLogger(categoryName, logLevels: logLevels, logLevelCacheByNamespace: logLevelCacheByNamespace);
		logger.IsEnabled(LogLevel.Information);
		logLevelCacheByNamespace.Count.Should().Be(1);
		logLevelCacheByNamespace.Single().Key.Should().Be(categoryName);
		logLevelCacheByNamespace.Single().Value.Should().Be(logLevel);
	}

	[Fact]
	public void IsEnabled_SecondInvocation_ShouldFetchFromCacheAndThereShouldBeNoDuplicateCacheEntry()
	{
		var categoryName = "RootNamespace1.TestClass1";
		var logLevel = LogLevel.Error;
		var logLevels = new ConcurrentDictionary<string, LogLevel>
		{
			[categoryName] = logLevel
		};
		var logLevelCacheByNamespace = new ConcurrentDictionary<string, LogLevel>();
		var logger = CreateLogger(categoryName, logLevels: logLevels, logLevelCacheByNamespace: logLevelCacheByNamespace);
		logger.IsEnabled(LogLevel.Information);
		logLevelCacheByNamespace.Count.Should().Be(1);
		logger.IsEnabled(LogLevel.Critical);
		logLevelCacheByNamespace.Count.Should().Be(1);
	}

	#endregion

	#region Log

	[Fact]
	public void Log_HigherLevelLoggingThanTheLoggerDefaultLogger_OutputShouldNonEmpty()
	{
		var (logger, console) = CreateLoggerWithConsole(defaultLogLevel: LogLevel.Information);
		logger.LogError("test message");
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().NotBeEmpty();
	}

	[Fact]
	public void Log_LowerLevelLoggingThanTheLoggerDefaultLogger_OutputShouldEmpty()
	{
		var (logger, console) = CreateLoggerWithConsole(defaultLogLevel: LogLevel.Critical);
		logger.LogError("test message");
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().BeEmpty();
	}

	[Fact]
	public void Log_BasicStringStatement_ShouldMatchWithOutput()
	{
		var (logger, console) = LoggerAndConsoleAlwaysEnabled();
		logger.LogError("test message");
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("test message");
	}

	[Fact]
	public void Log_StatementWithStringTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		logger.LogCritical("{StringVariable} test message", "value1");
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("value1 test message");
	}

	[Fact]
	public void Log_StatementWithIntTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		logger.LogCritical("{IntVariable} test message", 1);
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("1 test message");
	}

	[Fact]
	public void Log_StatementWithDoubleTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		logger.LogCritical("{DoubleVariable} test message", 1d);
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("1 test message");
	}

	[Fact]
	public void Log_StatementWithEnumTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		logger.LogCritical("{EnumVariable} test message", NamingStyle.AddressDateTimeWithMinutes);
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("AddressDateTimeWithMinutes test message");
	}

	[Fact]
	public void Log_StatementWithStringEnumerableTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		var stringEnumerable = new List<string> { "item1", "item2" };
		logger.LogCritical("{StringEnumerableVariable} test message", stringEnumerable);
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("item1, item2 test message");
	}

	[Fact]
	public void Log_StatementWithDictionaryKeyStringTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		var dictionary = new Dictionary<string, string>
		{
			["key1"] = "value1",
			["key2"] = "value2"
		};
		logger.LogCritical("{DictionaryVariable} test message", dictionary);
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("key1: value1, key2: value2 test message");
	}

	[Fact]
	public void Log_StatementWithDictionaryKeyIntTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		var dictionary = new Dictionary<int, string>
		{
			[1] = "value1",
			[2] = "value2"
		};
		logger.LogCritical("{DictionaryVariable} test message", dictionary);
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("1: value1, 2: value2 test message");
	}

	[Fact]
	public void Log_StatementWithObjectTemplateVariable_ShouldReplacedAndMatchWithOutput()
	{
		var (logger, console) = CreateLoggerWithConsole();
		var obj = new
		{
			P1 = 1,
			P2 = "2",
		};
		logger.LogCritical("{ObjectVariable} test message", obj);
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be($"{Environment.NewLine}{{\"P1\":1,\"P2\":\"2\"}}{Environment.NewLine} test message");
	}

	[Fact]
	public void Log_WithCategoryNameOutput_ShouldAppendedToMessage()
	{
		var (logger, console) = CreateLoggerWithConsole("Category1", logCategoryNameOutput: true);
		logger.LogError("test message");
		var actualOutput = ConsoleOutputCleanString(console);
		actualOutput.Should().Be("test message on (Category1)");
	}

	#endregion

	#region Helpers

	private static string NamespaceSample(int sampleId) => $"RootNamespace{sampleId}.TestClass{sampleId}";

	private string ConsoleOutputCleanString(TestConsole console)
	{
		return string.Join(Environment.NewLine, ConsoleOutputClean(console));
	}

	private IEnumerable<string> ConsoleOutputClean(TestConsole console)
	{
		return LogUtilities.RemoveTimeStamps(console.Lines);
	}

	private static (AnsiConsoleLogger, TestConsole) LoggerAndConsoleAlwaysEnabled()
	{
		return CreateLoggerWithConsole(defaultLogLevel: LogLevel.Trace);
	}

	private static AnsiConsoleLogger CreateLogger(string categoryName = "Test.Category", LogLevel defaultLogLevel = LogLevel.Information,
		ConcurrentDictionary<string, LogLevel>? logLevels = null, ConcurrentDictionary<string, LogLevel>? logLevelCacheByNamespace = null,
		bool logCategoryNameOutput = false)
	{
		var (logger, _) = CreateLoggerWithConsole(categoryName, defaultLogLevel, logLevels, logLevelCacheByNamespace, logCategoryNameOutput);
		return logger;
	}

	private static (AnsiConsoleLogger, TestConsole) CreateLoggerWithConsole(string categoryName = "Test.Category", LogLevel defaultLogLevel = LogLevel.Information,
		ConcurrentDictionary<string, LogLevel>? logLevels = null, ConcurrentDictionary<string, LogLevel>? logLevelCacheByNamespace = null,
		bool logCategoryNameOutput = false)
	{
		var testConsole = new TestConsole { Profile = { Width = 1000 } };

		var logger = new AnsiConsoleLogger(categoryName, defaultLogLevel,
			logLevels ?? new ConcurrentDictionary<string, LogLevel>(),
			logLevelCacheByNamespace ?? new ConcurrentDictionary<string, LogLevel>(),
			logCategoryNameOutput, new AnsiConsoleExtended(testConsole));

		return (logger, testConsole);
	}

	#endregion
}

#pragma warning restore CA2254
