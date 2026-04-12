#region Global using

global using System.Globalization;
global using System.Net;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using PhotoCli.Models;
global using PhotoCli.Models.Enums;
global using PhotoCli.Options;
global using PhotoCli.Options.Validators;
global using PhotoCli.Runners;
global using PhotoCli.Services.Contracts;
global using PhotoCli.Services.Contracts.ReverseGeocodes;
global using PhotoCli.Services.Implementations;
global using PhotoCli.Services.Implementations.ReverseGeocodes;
global using PhotoCli.Models.ReverseGeocode.BigDataCloud;
global using PhotoCli.Models.ReverseGeocode.GoogleMaps;
global using PhotoCli.Models.ReverseGeocode.OpenStreetMap;
global using PhotoCli.Utils;
global using PhotoCli.Utils.Extensions;
global using PhotoCli.Utils.Validators;
global using PhotoCli.Models.ReverseGeocode;
using System.IO.Abstractions;
using CommandLine;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using PhotoCli.McpTools;
using PhotoCli.Utils.Logging;
using Polly;
using Spectre.Console;
using ValidationResult = FluentValidation.Results.ValidationResult;

#endregion

namespace PhotoCli;

public static class Program
{
	public static Task<int> Main(string[] args)
	{
		return Main(args, AnsiConsole.Console);
	}

	public static Task<int> Main(IReadOnlyList<string> args, IAnsiConsole ansiConsole)
	{
		if (!ParseArgs(args, out var baseOptions, out var exitCode, ansiConsole))
			return ReturnExitCode(exitCode);

		IHost host;
		switch (baseOptions)
		{
			case AddressOptions reverseGeocodeOptions:
				{
					var validationResultReverseGeocode = new AddressOptionsValidator().Validate(reverseGeocodeOptions);
					if (!validationResultReverseGeocode.IsValid)
					{
						WriteErrorOutputValidationErrors(validationResultReverseGeocode);
						return ReturnExitCode(ExitCode.AddressOptionsValidationFailed);
					}

					host = BuildHostWithReverseGeocode<AddressRunner, AddressOptions>(reverseGeocodeOptions, false, ansiConsole);
					break;
				}
			case InfoOptions infoOptions:
				var validationResultInfo = new InfoOptionsValidator().Validate(infoOptions);
				if (!validationResultInfo.IsValid)
				{
					WriteErrorOutputValidationErrors(validationResultInfo);
					return ReturnExitCode(ExitCode.InfoOptionsValidationFailed);
				}
				host = BuildHostWithReverseGeocode<InfoRunner, InfoOptions>(infoOptions, false, ansiConsole);
				break;
			case CopyOptions copyOptions:
				{
					var validationResultCopy = new CopyOptionsValidator().Validate(copyOptions);
					if (!validationResultCopy.IsValid)
					{
						WriteErrorOutputValidationErrors(validationResultCopy);
						return ReturnExitCode(ExitCode.CopyOptionsValidationFailed);
					}
					host = BuildHostWithReverseGeocode<CopyRunner, CopyOptions>(copyOptions, false, ansiConsole);
					break;
				}
			case ArchiveOptions archiveOptions:
				{
					var validationResultCopy = new ArchiveOptionsValidator().Validate(archiveOptions);
					if (!validationResultCopy.IsValid)
					{
						WriteErrorOutputValidationErrors(validationResultCopy);
						return ReturnExitCode(ExitCode.ArchiveOptionsValidationFailed);
					}
					host = BuildHostWithReverseGeocode<ArchiveRunner, ArchiveOptions>(archiveOptions, true, ansiConsole, new ArchiveDatabaseOptions(archiveOptions.OutputPath));
					break;
				}
			case SettingsOptions settingsOptions:
				{
					var validationResultSettings = new SettingsOptionsValidator().Validate(settingsOptions);
					if (!validationResultSettings.IsValid)
					{
						WriteErrorOutputValidationErrors(validationResultSettings);
						return ReturnExitCode(ExitCode.SettingsOptionsValidationFailed);
					}
					host = BuildHost<SettingsRunner, SettingsOptions>(settingsOptions, ansiConsole);
					break;
				}
			case ListOptions listOptions:
				{
					var validationResultSettings = new ListOptionsValidator().Validate(listOptions);
					if (!validationResultSettings.IsValid)
					{
						WriteErrorOutputValidationErrors(validationResultSettings);
						return ReturnExitCode(ExitCode.SettingsOptionsValidationFailed);
					}
					host = BuildHost<ListRunner, ListOptions>(listOptions, ansiConsole, new ArchiveDatabaseOptions(listOptions.ArchivePath));
					break;
				}
			case McpOptions mcpOptions:
				{
					var validationResultMcp = new McpOptionsValidator().Validate(mcpOptions);
					if (!validationResultMcp.IsValid)
					{
						WriteErrorOutputValidationErrors(validationResultMcp);
						return ReturnExitCode(ExitCode.McpOptionsValidationFailed);
					}
					return RunMcpServer(mcpOptions);
				}
			default:
				throw new PhotoCliException($"Not defined: {baseOptions}");
		}

		using var serviceScope = host.Services.CreateScope();
		return MainWithServiceProvider(serviceScope.ServiceProvider);
	}

	private static void WriteErrorOutputValidationErrors(ValidationResult validationResult)
	{
		var ansiConsoleExtended = new AnsiConsoleExtended(AnsiConsole.Console);
		foreach (var validationResultError in validationResult.Errors)
			ansiConsoleExtended.WriteLine(validationResultError.ErrorMessage, Color.Red);
	}

	public static async Task<int> MainWithServiceProvider(IServiceProvider serviceProvider)
	{
		var consoleWriter = serviceProvider.GetRequiredService<IConsoleWriter>();
		var toolOptions = serviceProvider.GetRequiredService<ToolOptions>();
		var toolOptionsValidator = serviceProvider.GetRequiredService<IValidator<ToolOptions>>();

		var toolOptionsValidationResult = toolOptionsValidator.Validate(toolOptions);
		if (!toolOptionsValidationResult.IsValid)
		{
			consoleWriter.WriteError($"{Constants.AppSettingsFileName} has some invalid settings. Undo or reset all settings via `{OptionNames.ApplicationAlias} {OptionNames.SettingsVerb} --{OptionNames.ResetOptionNameLong}`");
			foreach (var validationResultError in toolOptionsValidationResult.Errors)
				consoleWriter.WriteError(validationResultError.ToString());
			return (int)ExitCode.AppSettingsInvalidFile;
		}

		ServicePointManager.DefaultConnectionLimit = toolOptions.ConnectionLimit;

		var apiKeyStore = serviceProvider.GetRequiredService<ApiKeyStore>();
		var apiKeyStoreValidationResult = new ApiKeyStoreValidator().Validate(apiKeyStore);
		if (!apiKeyStoreValidationResult.IsValid)
		{
			foreach (var validationResultError in apiKeyStoreValidationResult.Errors)
				Console.Error.WriteLine(validationResultError);
			return (int)ExitCode.ApiKeyStoreValidationFailed;
		}

		var consoleRunner = serviceProvider.GetRequiredService<IConsoleRunner>();
		var logger = serviceProvider.GetRequiredService<ILogger<IConsoleRunner>>();
		ExitCode exitCode;
		try
		{
			exitCode = await AnsiConsole.Status()
				.Spinner(Spinner.Known.Dots2)
				.SpinnerStyle(Style.Parse("blue"))
				.StartAsync("-",
					async spectreStatusContext =>
					{
						consoleWriter.InitializeSpectreContext(spectreStatusContext);
						return await consoleRunner.Execute();
					});
		}
		catch (Exception e)
		{
			logger.LogCritical(e, "Unhandled exception");
			exitCode = ExitCode.UnexpectedError;
		}
		var exitCodeValue = (int)exitCode;
		if (exitCode != ExitCode.Success)
			consoleWriter.WriteError($"Process failed with a error code {exitCodeValue} ({exitCode})");
		else
			logger.LogInformation("{Type}, exists with code {ProcessCode} ({ExitCodeEnum})", consoleRunner.GetType().Name, exitCodeValue, exitCode);
		return exitCodeValue;
	}

	public static IHost BuildHost<TConsoleRunner, TOptions>(TOptions options, IAnsiConsole ansiConsole, ArchiveDatabaseOptions? archiveDatabaseOptions = null)
		where TOptions : class where TConsoleRunner : IConsoleRunner
	{
		return BuildHostCore<TConsoleRunner>((services, _) =>
		{
			services.AddSingleton(options);
			services.AddSingleton(new ApiKeyStore());
		}, ansiConsole, archiveDatabaseOptions);
	}

	public static IHost BuildHostWithReverseGeocode<TConsoleRunner, TOptions>(TOptions options, bool useDbReverseGeocodeCache, IAnsiConsole ansiConsole,
		ArchiveDatabaseOptions? archiveDatabaseOptions = null) where TOptions : class, IReverseGeocodeOptions where TConsoleRunner : IConsoleRunner
	{
		return BuildHostCore<TConsoleRunner>((services, configuration) =>
		{
			services.AddSingleton<IReverseGeocodeOptions>(options);
			services.AddSingleton(options);

			#region ReverseGeocode Providers

			var apiKeyStore = ApiKeyStore.Build(configuration, options);
			services.AddSingleton(apiKeyStore);

			if (useDbReverseGeocodeCache)
			{
				services.AddSingleton<IReverseGeocodeCache<BigDataCloudResponse>, ReverseGeocodeCacheDatabase<BigDataCloudResponse>>();
				services.AddSingleton<IReverseGeocodeCache<GoogleMapsResponse>, ReverseGeocodeCacheDatabase<GoogleMapsResponse>>();
				services.AddSingleton<IReverseGeocodeCache<OpenStreetMapResponse>, ReverseGeocodeCacheDatabase<OpenStreetMapResponse>>();
			}
			else
			{
				services.AddSingleton<IReverseGeocodeCache<BigDataCloudResponse>, ReverseGeocodeCacheMemory<BigDataCloudResponse>>();
				services.AddSingleton<IReverseGeocodeCache<GoogleMapsResponse>, ReverseGeocodeCacheMemory<GoogleMapsResponse>>();
				services.AddSingleton<IReverseGeocodeCache<OpenStreetMapResponse>, ReverseGeocodeCacheMemory<OpenStreetMapResponse>>();
			}

			var agent = UserAgent.Instance();

			services.AddHttpClient<IBigDataCloudReverseGeocodeService, BigDataCloudReverseGeocodeService>(c =>
			{
				c.BaseAddress = new Uri("https://api.bigdatacloud.net/data/reverse-geocode");
				c.DefaultRequestHeaders.UserAgent.Add(agent);
			}).AddResilience();

			services.AddHttpClient<IOpenStreetMapFoundationReverseGeocodeService, OpenStreetMapFoundationReverseGeocodeService>(c =>
			{
				c.BaseAddress = new Uri("https://nominatim.openstreetmap.org/reverse");
				c.DefaultRequestHeaders.UserAgent.Add(agent);
			}).AddResilience();

			services.AddHttpClient<IGoogleMapsReverseGeocodeService, GoogleMapsReverseGeocodeService>(c =>
			{
				c.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/geocode/json");
				c.DefaultRequestHeaders.UserAgent.Add(agent);
			}).AddResilience();

			services.AddHttpClient<ILocationIqReverseGeocodeService, LocationIqReverseGeocodeService>(c =>
			{
				c.BaseAddress = new Uri("https://us1.locationiq.com/v1/reverse.php");
				c.DefaultRequestHeaders.UserAgent.Add(agent);
			}).AddResilience();

			#endregion
		}, ansiConsole, archiveDatabaseOptions);
	}

	private static IHttpClientBuilder AddResilience(this IHttpClientBuilder httpClientBuilder)
	{
		httpClientBuilder.AddResilienceHandler("retry-timeout-pipeline", builder =>
		{
			builder.AddRetry(new HttpRetryStrategyOptions
			{
				MaxRetryAttempts = 10,
				BackoffType = DelayBackoffType.Exponential
			});

			builder.AddTimeout(TimeSpan.FromSeconds(5));
		});
		return httpClientBuilder;
	}

	private static IHost BuildHostCore<TConsoleRunner>(Action<IServiceCollection, IConfigurationRoot>? additionalConfigureServices, IAnsiConsole ansiConsole,
		ArchiveDatabaseOptions? archiveDatabaseOptions = null) where TConsoleRunner : IConsoleRunner
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

		var builder = new HostBuilder().ConfigureServices(services =>
		{
			var configuration = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.AddJsonFile(Constants.AppSettingsFileName, true)
				.Build();

			services.Configure<ToolOptionsRaw>(configuration);

			var toolOptionsRaw = configuration.Get<ToolOptionsRaw>() ?? new ToolOptionsRaw();
			var toolOptions = new ToolOptions(toolOptionsRaw);

			services.AddTransient(typeof(IConsoleRunner), typeof(TConsoleRunner));
			services.AddTransient<IFileSystem, FileSystem>();
			services.AddTransient<IExifParserService, ExifParserService>();
			services.AddTransient<IExifDataAppenderService, ExifDataAppenderService>();
			services.AddTransient<IPhotoCollectorService, PhotoCollectorService>();
			services.AddTransient<IDirectoryGrouperService, DirectoryGrouperService>();
			services.AddTransient<IFileNamerService, FileNamerService>();
			services.AddTransient<IFileService, FileService>();
			services.AddTransient<ICsvService, CsvService>();
			services.AddTransient<ISequentialNumberEnumeratorService, SequentialNumberEnumeratorService>();
			services.AddTransient<IExifOrganizerService, ExifOrganizerService>();
			services.AddTransient<IExifDataGrouperService, ExifDataGrouperService>();
			services.AddTransient<IFolderRenamerService, FolderRenamerService>();
			services.AddTransient<IReverseGeocodeService, ReverseGeocodeService>();
			services.AddTransient<IReverseGeocodeFetcherService, ReverseGeocodeFetcherService>();
			services.AddTransient<IValidator<ToolOptions>, ToolOptionsValidator>();
			services.AddTransient<IDuplicatePhotoRemoveService, DuplicatePhotoRemoveService>();
			services.AddTransient<IDbService, DbService>();
			services.AddTransient<IProcessLauncher, ProcessLauncher>();

			services.AddSingleton(configuration);
			services.AddSingleton(toolOptions);
			services.AddSingleton<IArchiveDbContextProvider, ArchiveDbContextProvider>();
			services.AddSingleton<ISQLiteConnectionStringProvider, ArchiveIsqLiteConnectionStringProvider>();
			services.AddSingleton<IConsoleWriter, ConsoleWriter>();
			services.AddSingleton<Statistics>();
			services.AddSingleton<ILoggerProvider, AnsiConsoleLoggerProvider>();
			services.AddSingleton(ansiConsole);
			services.AddSingleton<AnsiConsoleExtended>();

			additionalConfigureServices?.Invoke(services, configuration);

			if (archiveDatabaseOptions != null)
				services.AddSingleton(archiveDatabaseOptions);
		});
		return builder.UseConsoleLifetime().Build();
	}

	private static bool ParseArgs(IReadOnlyList<string> args, out object parsedObject, out ExitCode exitCode, IAnsiConsole ansiConsole)
	{
		var commandLineArgsParsed = Parser.Default.ParseArguments<CopyOptions, InfoOptions, ArchiveOptions, AddressOptions, SettingsOptions, ListOptions, McpOptions>(args);
		if (commandLineArgsParsed.Tag == ParserResultType.NotParsed)
		{
			var notParsedResult = (NotParsed<object>)commandLineArgsParsed;
			if (notParsedResult.Errors.IsHelp())
			{
				switch (args.Count)
				{
					case 1:
						HelpTextBuilder.ExtendedHelpWritingToConsole(ansiConsole);
						break;
					case 2:
						var helpVerb = args[1];
						// Can't use CommandLineParser's UsageExamples when using Nullable Reference Types. ref: https://github.com/commandlineparser/commandline/issues/714 , we are building on our own.
						HelpTextBuilder.ExampleUsages(helpVerb, ansiConsole);
						break;
				}

				exitCode = ExitCode.Success;
			}
			else
			{
				if (args.Count == 0)
					HelpTextBuilder.ExtendedHelpWritingToConsole(ansiConsole);
				exitCode = ExitCode.ParseArgsFailed;
			}
			parsedObject = null!;
			return false;
		}

		parsedObject = ((Parsed<object>)commandLineArgsParsed).Value;
		exitCode = ExitCode.Unset;
		return true;
	}

	private static Task<int> ReturnExitCode(ExitCode exitCode)
	{
		return Task.FromResult((int)exitCode);
	}

	private static async Task<int> RunMcpServer(McpOptions options)
	{
		var dbPath = Path.Combine(options.ArchivePath, Constants.ArchiveSQLiteDatabaseFileName);
		var builder = Host.CreateApplicationBuilder();
		builder.Logging.ClearProviders();
		builder.Services.AddDbContext<ArchiveDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));
		builder.Services.AddScoped<IArchiveDbContextProvider, McpArchiveDbContextProvider>();
		builder.Services.AddSingleton(ToolOptions.Default());
		builder.Services.AddSingleton(options);
		builder.Services.AddSingleton(new Statistics());
		builder.Services.AddSingleton<IConsoleWriter, NullConsoleWriter>();
		builder.Services.AddSingleton<IProcessLauncher, ProcessLauncher>();
		builder.Services.AddScoped<IDbService, DbService>();

		builder.Services
			.AddMcpServer()
			.WithStdioServerTransport()
			.WithTools<ArchiveMcpTools>();

		await builder.Build().RunAsync();
		return (int)ExitCode.Success;
	}
}
