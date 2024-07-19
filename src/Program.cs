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
using FluentValidation.Results;
using Microsoft.Extensions.Http.Resilience;
using Polly;

#endregion

namespace PhotoCli;

public static class Program
{
	public static Task<int> Main(string[] args)
	{
		return MainStream(args, Console.Out);
	}

	public static Task<int> MainStream(string[] args, TextWriter textWriter)
	{
		if (!ParseArgs(args, textWriter, out var baseOptions, out var exitCode))
			return ReturnExitCode(exitCode);

		IHost host;
		switch (baseOptions)
		{
			case AddressOptions reverseGeocodeOptions:
			{
				var validationResultReverseGeocode = new AddressOptionsValidator().Validate(reverseGeocodeOptions);
				if (!validationResultReverseGeocode.IsValid)
				{
					WriteErrorOutputValidationErrors(validationResultReverseGeocode, textWriter);
					return ReturnExitCode(ExitCode.AddressOptionsValidationFailed);
				}

				host = BuildHostWithReverseGeocode<AddressRunner, AddressOptions>(reverseGeocodeOptions, textWriter);
				break;
			}
			case InfoOptions infoOptions:
				var validationResultInfo = new InfoOptionsValidator().Validate(infoOptions);
				if (!validationResultInfo.IsValid)
				{
					WriteErrorOutputValidationErrors(validationResultInfo, textWriter);
					return ReturnExitCode(ExitCode.InfoOptionsValidationFailed);
				}

				host = BuildHostWithReverseGeocode<InfoRunner, InfoOptions>(infoOptions, textWriter);
				break;
			case CopyOptions copyOptions:
			{
				var validationResultCopy = new CopyOptionsValidator().Validate(copyOptions);
				if (!validationResultCopy.IsValid)
				{
					WriteErrorOutputValidationErrors(validationResultCopy, textWriter);
					return ReturnExitCode(ExitCode.CopyOptionsValidationFailed);
				}

				host = BuildHostWithReverseGeocode<CopyRunner, CopyOptions>(copyOptions, textWriter);
				break;
			}
			case ArchiveOptions archiveOptions:
			{
				var validationResultCopy = new ArchiveOptionsValidator().Validate(archiveOptions);
				if (!validationResultCopy.IsValid)
				{
					WriteErrorOutputValidationErrors(validationResultCopy, textWriter);
					return ReturnExitCode(ExitCode.ArchiveOptionsValidationFailed);
				}

				host = BuildHostWithReverseGeocode<ArchiveRunner, ArchiveOptions>(archiveOptions, textWriter);
				break;
			}
			case SettingsOptions settingsOptions:
			{
				var validationResultSettings = new SettingsOptionsValidator().Validate(settingsOptions);
				if (!validationResultSettings.IsValid)
				{
					WriteErrorOutputValidationErrors(validationResultSettings, textWriter);
					return ReturnExitCode(ExitCode.SettingsOptionsValidationFailed);
				}

				host = BuildHost<SettingsRunner, SettingsOptions>(settingsOptions, textWriter);
				break;
			}
			default:
				throw new PhotoCliException($"Not defined: {baseOptions}");
		}

		using var serviceScope = host.Services.CreateScope();
		return MainWithServiceProvider(serviceScope.ServiceProvider);
	}

	private static void WriteErrorOutputValidationErrors(ValidationResult validationResult, TextWriter textWriter)
	{
		foreach (var validationResultError in validationResult.Errors)
			textWriter.WriteLine(validationResultError);
	}

	public static async Task<int> MainWithServiceProvider(IServiceProvider serviceProvider)
	{
		var toolOptions = serviceProvider.GetRequiredService<ToolOptions>();
		var toolOptionsValidator = serviceProvider.GetRequiredService<IValidator<ToolOptions>>();

		var toolOptionsValidationResult = toolOptionsValidator.Validate(toolOptions);
		if (!toolOptionsValidationResult.IsValid)
		{
			Console.Error.WriteLine($"{Constants.AppSettingsFileName} has some invalid settings. Undo or reset all settings via `{OptionNames.ApplicationAlias} {OptionNames.SettingsVerb} --{OptionNames.ResetOptionNameLong}`");
			foreach (var validationResultError in toolOptionsValidationResult.Errors)
				Console.Error.WriteLine(validationResultError);
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
		var consoleWriter = serviceProvider.GetRequiredService<IConsoleWriter>();
		var exitCode = await consoleRunner.Execute();
		var exitCodeValue = (int)exitCode;
		if (exitCode != ExitCode.Success)
			consoleWriter.Write($"Process failed with a error code {exitCodeValue} ({exitCode})");
		else
			logger.LogInformation("{Type}, exists with code {ProcessCode} ({ExitCodeEnum})", consoleRunner.GetType().Name, exitCodeValue, exitCode);

		return exitCodeValue;
	}

	public static IHost BuildHost<TConsoleRunner, TOptions>(TOptions options, TextWriter textWriter) where TOptions : class where TConsoleRunner : IConsoleRunner
	{
		return BuildHostCore<TConsoleRunner>(textWriter, (services, _) =>
		{
			services.AddSingleton(options);
			services.AddSingleton(new ApiKeyStore());
		});
	}

	public static IHost BuildHostWithReverseGeocode<TConsoleRunner, TOptions>(TOptions options, TextWriter textWriter)
		where TOptions : class, IReverseGeocodeOptions where TConsoleRunner : IConsoleRunner
	{
		return BuildHostCore<TConsoleRunner>(textWriter, (services, configuration) =>
		{
			services.AddSingleton<IReverseGeocodeOptions>(options);
			services.AddSingleton(options);

			#region ReverseGeocode Providers

			var apiKeyStore = ApiKeyStore.Build(configuration, options);
			services.AddSingleton(apiKeyStore);

			services.AddSingleton<ICoordinateCache<BigDataCloudResponse>, CoordinateCache<BigDataCloudResponse>>();
			services.AddSingleton<ICoordinateCache<GoogleMapsResponse>, CoordinateCache<GoogleMapsResponse>>();
			services.AddSingleton<ICoordinateCache<OpenStreetMapResponse>, CoordinateCache<OpenStreetMapResponse>>();

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
		});
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

	private static IHost BuildHostCore<TConsoleRunner>(TextWriter textWriter, Action<IServiceCollection, IConfigurationRoot>? additionalConfigureServices) where TConsoleRunner : IConsoleRunner
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

		var builder = new HostBuilder().ConfigureServices(services =>
		{
			var configuration = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.AddJsonFile(Constants.AppSettingsFileName, true)
				.Build();

			services.Configure<ToolOptionsRaw>(configuration);

			services.AddLogging(o =>
			{
				o.AddConsole();
				o.AddDebug();
				o.AddConfiguration(configuration);
			});

			services.AddSingleton(configuration);
			var toolOptionsRaw = configuration.Get<ToolOptionsRaw>() ?? new ToolOptionsRaw();
			var toolOptions = new ToolOptions(toolOptionsRaw);
			services.AddSingleton(toolOptions);
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
			services.AddSingleton<IArchiveDbContextProvider, ArchiveDbContextProvider>();
			services.AddSingleton<ISQLiteConnectionStringProvider, ArchiveIsqLiteConnectionStringProvider>();

			services.AddSingleton(textWriter);
			services.AddSingleton<IConsoleWriter, ConsoleWriter>();
			services.AddSingleton<Statistics>();

			additionalConfigureServices?.Invoke(services, configuration);
		});
		return builder.UseConsoleLifetime().Build();
	}

	private static bool ParseArgs(IReadOnlyList<string> args, TextWriter textWriter, out object parsedObject, out ExitCode exitCode)
	{
		var commandLineArgsParsed = Parser.Default.ParseArguments<CopyOptions, InfoOptions, ArchiveOptions, AddressOptions, SettingsOptions>(args);
		if (commandLineArgsParsed.Tag == ParserResultType.NotParsed)
		{
			var notParsedResult = (NotParsed<object>)commandLineArgsParsed;
			if (notParsedResult.Errors.IsHelp())
			{
				switch (args.Count)
				{
					case 1:
						HelpTextBuilder.ExtendedHelpWritingToConsole(textWriter);
						break;
					case 2:
						var helpVerb = args[1];
						// Can't use CommandLineParser's UsageExamples when using Nullable Reference Types. ref: https://github.com/commandlineparser/commandline/issues/714 , we are building on our own.
						HelpTextBuilder.ExampleUsages(helpVerb, textWriter);
						break;
				}

				exitCode = ExitCode.Success;
			}
			else
			{
				if (args.Count == 0)
					HelpTextBuilder.ExtendedHelpWritingToConsole(textWriter);
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
}
