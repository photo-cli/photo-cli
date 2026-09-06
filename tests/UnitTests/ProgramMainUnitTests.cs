using System.IO.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Polly.Timeout;
using Spectre.Console;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PhotoCli.Tests.UnitTests;

public class StartupTests
{
	[Fact]
	public async Task Commandline_Arguments_Which_Cant_Parsed_Should_Exists_With_ParseArgsFailed()
	{
		var exitCode = (ExitCode)await Program.Main(CommandLineArgumentsFakes.NotParseable());
		exitCode.Should().Be(ExitCode.ParseArgsFailed);
	}

	[Fact]
	public async Task CopyOptions_Invalid_Should_Exit_With_CopyOptionsValidationFailed()
	{
		await ProgramMainShouldExitWithValidationFailed(CommandLineArgumentsFakes.CopyOptionsInvalid(), ExitCode.CopyOptionsValidationFailed);
	}

	[Fact]
	public async Task InfoOptions_Invalid_Should_Exit_With_InfoOptionsValidationFailed()
	{
		await ProgramMainShouldExitWithValidationFailed(CommandLineArgumentsFakes.InfoOptionsInvalid(), ExitCode.InfoOptionsValidationFailed);
	}

	[Fact]
	public async Task AddressOptions_Invalid_Should_Exit_With_AddressOptionsValidationFailed()
	{
		await ProgramMainShouldExitWithValidationFailed(CommandLineArgumentsFakes.AddressOptionsInvalid(), ExitCode.AddressOptionsValidationFailed);
	}

	[Fact]
	public async Task SettingsOptions_Invalid_Should_Exit_With_SettingsOptionsValidationFailed()
	{
		await ProgramMainShouldExitWithValidationFailed(CommandLineArgumentsFakes.SettingsOptionsInvalid(), ExitCode.SettingsOptionsValidationFailed);
	}

	[Fact]
	public async Task ArchiveOptions_Invalid_Should_Exit_With_InfoOptionsValidationFailed()
	{
		await ProgramMainShouldExitWithValidationFailed(CommandLineArgumentsFakes.ArchiveOptionsInvalid(), ExitCode.ArchiveOptionsValidationFailed);
	}

	private async Task ProgramMainShouldExitWithValidationFailed(string[] args, ExitCode expectedExitCode)
	{
		var processReturnCode = await Program.Main(args);
		var actualExitCode = (ExitCode)processReturnCode;
		actualExitCode.Should().Be(expectedExitCode);
	}

	[Theory]
	[InlineData(ExitCode.Success)]
	[InlineData(ExitCode.ParseArgsFailed)]
	public async Task ProgramMain_Returns_Logic_Exit_Code(ExitCode expectedExitCode)
	{
		var consoleRunnerMock = new Mock<IConsoleRunner>();
		consoleRunnerMock.Setup(s => s.Execute()).ReturnsAsync(expectedExitCode);
		var consoleWriterMock = new Mock<IConsoleWriter>();
		consoleWriterMock.Setup(s => s.Write(It.IsAny<string>()));

		var serviceProvider = new Mock<IServiceProvider>();
		MockToolOptionsValidator(serviceProvider, false);
		serviceProvider.Setup(s => s.GetService(typeof(IConsoleRunner))).Returns(consoleRunnerMock.Object);
		serviceProvider.Setup(s => s.GetService(typeof(IConsoleWriter))).Returns(consoleWriterMock.Object);
		serviceProvider.Setup(s => s.GetService(typeof(ApiKeyStore))).Returns(ApiKeyStoreFakes.Valid);
		serviceProvider.Setup(e => e.GetService(typeof(ILogger<IConsoleRunner>))).Returns(() => NullLogger<IConsoleRunner>.Instance);

		var sutExitCodeActual = (ExitCode)await Program.MainWithServiceProvider(serviceProvider.Object);
		consoleRunnerMock.Verify(e => e.Execute(), Times.Once);
		consoleRunnerMock.VerifyNoOtherCalls();
		sutExitCodeActual.Should().Be(expectedExitCode);
	}

	private void MockToolOptionsValidator(Mock<IServiceProvider> serviceProviderMock, bool hasError)
	{
		var validationResult = ValidationResultFakes.Get(hasError);
		var toolOptionsValidatorMock = new Mock<IValidator<ToolOptions>>();
		toolOptionsValidatorMock.Setup(s => s.Validate(It.IsAny<ToolOptions>())).Returns(validationResult);
		serviceProviderMock.Setup(e => e.GetService(typeof(IConsoleWriter))).Returns(new Mock<IConsoleWriter>().Object);
		serviceProviderMock.Setup(e => e.GetService(typeof(ToolOptions))).Returns(ToolOptionsFakes.Valid);
		serviceProviderMock.Setup(e => e.GetService(typeof(IValidator<ToolOptions>))).Returns(() => toolOptionsValidatorMock.Object);
	}

	[Fact]
	public async Task Invalid_ToolOptions_Should_Exit_With_AppSettingsInvalidFile()
	{
		var serviceProvider = new Mock<IServiceProvider>();
		MockToolOptionsValidator(serviceProvider, true);
		var sutExitCodeActual = (ExitCode)await Program.MainWithServiceProvider(serviceProvider.Object);
		sutExitCodeActual.Should().Be(ExitCode.AppSettingsInvalidFile);
	}

	[Fact]
	public async Task Invalid_ApiKeyStore_Should_Exit_With_ApiKeyStoreValidationFailed()
	{
		var serviceProvider = new Mock<IServiceProvider>();
		MockToolOptionsValidator(serviceProvider, false);
		serviceProvider.Setup(s => s.GetService(typeof(ApiKeyStore))).Returns(ApiKeyStoreFakes.Invalid);
		var sutExitCodeActual = (ExitCode)await Program.MainWithServiceProvider(serviceProvider.Object);
		sutExitCodeActual.Should().Be(ExitCode.ApiKeyStoreValidationFailed);
	}

	public static TheoryData<Exception> Exceptions = new()
	{
		new Exception(),
		new Exception("custom basic exception"),
		new ArgumentException(),
		new AggregateException(),
		new TimeoutRejectedException(),
		new DbUpdateException(),
	};

	[Theory]
	[MemberData(nameof(Exceptions))]
	public async Task ConsoleRunnerExecute_UnhandledException_ShouldExitWithCodeUnexpectedErrorAndOutputTheErrorOnConsole(Exception exception)
	{
		var consoleRunnerMock = new Mock<IConsoleRunner>();
		consoleRunnerMock.Setup(s => s.Execute()).ThrowsAsync(exception);
		var consoleWriterMock = new Mock<IConsoleWriter>();
		consoleWriterMock.Setup(s => s.Write(It.IsAny<string>()));

		var serviceProvider = new Mock<IServiceProvider>();
		MockToolOptionsValidator(serviceProvider, false);
		serviceProvider.Setup(s => s.GetService(typeof(IConsoleRunner))).Returns(consoleRunnerMock.Object);
		serviceProvider.Setup(s => s.GetService(typeof(IConsoleWriter))).Returns(consoleWriterMock.Object);
		serviceProvider.Setup(s => s.GetService(typeof(ApiKeyStore))).Returns(ApiKeyStoreFakes.Valid);
		var loggerConsoleRunnerMock = new Mock<ILogger<IConsoleRunner>>();
		serviceProvider.Setup(e => e.GetService(typeof(ILogger<IConsoleRunner>))).Returns(() => loggerConsoleRunnerMock.Object);

		var sutExitCodeActual = (ExitCode)await Program.MainWithServiceProvider(serviceProvider.Object);

		using (new AssertionScope())
		{
			sutExitCodeActual.Should().Be(ExitCode.UnexpectedError);
			loggerConsoleRunnerMock.VerifyExceptionLogStatement(LogLevel.Critical, exception, "Unhandled exception", true);
		}
	}

	#region Get Service

	[Theory]
	[InlineData(typeof(CopyOptions))]
	[InlineData(typeof(IConsoleRunner))]
	[InlineData(typeof(IFileSystem))]
	[InlineData(typeof(IExifParserService))]
	[InlineData(typeof(IExifDataAppenderService))]
	[InlineData(typeof(IPhotoCollectorService))]
	[InlineData(typeof(IDirectoryGrouperService))]
	[InlineData(typeof(IFileNamerService))]
	[InlineData(typeof(IFileService))]
	[InlineData(typeof(ICsvService))]
	[InlineData(typeof(ISequentialNumberEnumeratorService))]
	[InlineData(typeof(IExifOrganizerService))]
	[InlineData(typeof(IExifDataGrouperService))]
	[InlineData(typeof(IFolderRenamerService))]
	[InlineData(typeof(IReverseGeocodeFetcherService))]
	[InlineData(typeof(IValidator<ToolOptions>))]
	[InlineData(typeof(IConsoleWriter))]
	[InlineData(typeof(IBigDataCloudReverseGeocodeService))]
	[InlineData(typeof(IGoogleMapsReverseGeocodeService))]
	[InlineData(typeof(IOpenStreetMapFoundationReverseGeocodeService))]
	[InlineData(typeof(ILocationIqReverseGeocodeService))]
	[InlineData(typeof(IReverseGeocodeCache<BigDataCloudResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<GoogleMapsResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<OpenStreetMapResponse>))]
	[InlineData(typeof(ILoggerProvider))]
	[InlineData(typeof(IAnsiConsole))]
	[InlineData(typeof(AnsiConsoleExtended))]
	public void CopyRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHostWithReverseGeocode<CopyRunner, CopyOptions>(CopyOptionsFakes.Valid(), false, AnsiConsole.Console);
		var sut = host.Services.CreateScope().ServiceProvider;
		var service = sut.GetService(type);
		service.Should().NotBeNull();
	}

	[Theory]
	[InlineData(typeof(InfoOptions))]
	[InlineData(typeof(IConsoleRunner))]
	[InlineData(typeof(IFileSystem))]
	[InlineData(typeof(IExifParserService))]
	[InlineData(typeof(IExifDataAppenderService))]
	[InlineData(typeof(IPhotoCollectorService))]
	[InlineData(typeof(IDirectoryGrouperService))]
	[InlineData(typeof(IFileNamerService))]
	[InlineData(typeof(IFileService))]
	[InlineData(typeof(ICsvService))]
	[InlineData(typeof(ISequentialNumberEnumeratorService))]
	[InlineData(typeof(IExifDataGrouperService))]
	[InlineData(typeof(IFolderRenamerService))]
	[InlineData(typeof(IReverseGeocodeFetcherService))]
	[InlineData(typeof(IValidator<ToolOptions>))]
	[InlineData(typeof(IConsoleWriter))]
	[InlineData(typeof(IBigDataCloudReverseGeocodeService))]
	[InlineData(typeof(IGoogleMapsReverseGeocodeService))]
	[InlineData(typeof(IOpenStreetMapFoundationReverseGeocodeService))]
	[InlineData(typeof(ILocationIqReverseGeocodeService))]
	[InlineData(typeof(IReverseGeocodeCache<BigDataCloudResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<GoogleMapsResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<OpenStreetMapResponse>))]
	[InlineData(typeof(ILoggerProvider))]
	[InlineData(typeof(IAnsiConsole))]
	[InlineData(typeof(AnsiConsoleExtended))]
	public void InfoRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHostWithReverseGeocode<InfoRunner, InfoOptions>(InfoOptionsFakes.Valid(), false, SpectreConsoleFakes.Actual);
		var sut = host.Services.CreateScope().ServiceProvider;
		var service = sut.GetService(type);
		service.Should().NotBeNull();
	}

	[Theory]
	[InlineData(typeof(AddressOptions))]
	[InlineData(typeof(IConsoleRunner))]
	[InlineData(typeof(IFileSystem))]
	[InlineData(typeof(IExifParserService))]
	[InlineData(typeof(IReverseGeocodeFetcherService))]
	[InlineData(typeof(IValidator<ToolOptions>))]
	[InlineData(typeof(IConsoleWriter))]
	[InlineData(typeof(IBigDataCloudReverseGeocodeService))]
	[InlineData(typeof(IGoogleMapsReverseGeocodeService))]
	[InlineData(typeof(IOpenStreetMapFoundationReverseGeocodeService))]
	[InlineData(typeof(ILocationIqReverseGeocodeService))]
	[InlineData(typeof(IReverseGeocodeCache<BigDataCloudResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<GoogleMapsResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<OpenStreetMapResponse>))]
	[InlineData(typeof(ILoggerProvider))]
	[InlineData(typeof(IAnsiConsole))]
	[InlineData(typeof(AnsiConsoleExtended))]
	public void AddressRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHostWithReverseGeocode<AddressRunner, AddressOptions>(AddressOptionsFakes.Valid(), false, SpectreConsoleFakes.Actual);
		var sut = host.Services.CreateScope().ServiceProvider;
		var service = sut.GetService(type);
		service.Should().NotBeNull();
	}

	[Theory]
	[InlineData(typeof(SettingsOptions))]
	[InlineData(typeof(IConsoleRunner))]
	[InlineData(typeof(IFileSystem))]
	[InlineData(typeof(IValidator<ToolOptions>))]
	[InlineData(typeof(IConsoleWriter))]
	[InlineData(typeof(ILoggerProvider))]
	[InlineData(typeof(IAnsiConsole))]
	[InlineData(typeof(AnsiConsoleExtended))]
	public void SettingsRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHost<SettingsRunner, SettingsOptions>(SettingsOptionsFakes.Valid(), SpectreConsoleFakes.Actual);
		var sut = host.Services.CreateScope().ServiceProvider;
		var service = sut.GetService(type);
		service.Should().NotBeNull();
	}

	[Theory]
	[InlineData(typeof(ArchiveOptions))]
	[InlineData(typeof(IConsoleRunner))]
	[InlineData(typeof(IFileSystem))]
	[InlineData(typeof(IExifParserService))]
	[InlineData(typeof(IExifDataAppenderService))]
	[InlineData(typeof(IPhotoCollectorService))]
	[InlineData(typeof(IDirectoryGrouperService))]
	[InlineData(typeof(IFileNamerService))]
	[InlineData(typeof(IFileService))]
	[InlineData(typeof(IExifDataGrouperService))]
	[InlineData(typeof(IFolderRenamerService))]
	[InlineData(typeof(IReverseGeocodeFetcherService))]
	[InlineData(typeof(IValidator<ToolOptions>))]
	[InlineData(typeof(IConsoleWriter))]
	[InlineData(typeof(IBigDataCloudReverseGeocodeService))]
	[InlineData(typeof(IGoogleMapsReverseGeocodeService))]
	[InlineData(typeof(IOpenStreetMapFoundationReverseGeocodeService))]
	[InlineData(typeof(ILocationIqReverseGeocodeService))]
	[InlineData(typeof(IDuplicatePhotoRemoveService))]
	[InlineData(typeof(IReverseGeocodeCache<BigDataCloudResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<GoogleMapsResponse>))]
	[InlineData(typeof(IReverseGeocodeCache<OpenStreetMapResponse>))]
	[InlineData(typeof(ILoggerProvider))]
	[InlineData(typeof(IAnsiConsole))]
	[InlineData(typeof(AnsiConsoleExtended))]
	public void GetService_BuildingArchiveRunner_ShouldResolveService(Type type)
	{
		var archiveDatabaseOptions = ArchiveDatabaseOptionsFakes.Valid();
		var host = Program.BuildHostWithReverseGeocode<ArchiveRunner, ArchiveOptions>(ArchiveOptionsFakes.Valid(), true, SpectreConsoleFakes.Actual, _ => archiveDatabaseOptions);
		var sut = host.Services.CreateScope().ServiceProvider;
		var service = sut.GetService(type);
		service.Should().NotBeNull();
	}

	#endregion
}
