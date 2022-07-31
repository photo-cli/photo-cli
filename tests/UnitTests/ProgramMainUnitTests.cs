using System.IO.Abstractions;
using FluentValidation;

namespace PhotoCli.Tests.UnitTests;

[Collection(XunitSharedCollectionsToDisableParallelExecution.AppSettingsJson)]
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
	[InlineData(typeof(IMapQuestReverseGeocodeService))]
	[InlineData(typeof(ILocationIqReverseGeocodeService))]
	public void CopyRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHostWithReverseGeocode<CopyRunner, CopyOptions>(CopyOptionsFakes.Valid(), TextWriterFakes.Valid());
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
	[InlineData(typeof(IMapQuestReverseGeocodeService))]
	[InlineData(typeof(ILocationIqReverseGeocodeService))]
	public void InfoRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHostWithReverseGeocode<InfoRunner, InfoOptions>(InfoOptionsFakes.Valid(), TextWriterFakes.Valid());
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
	[InlineData(typeof(IMapQuestReverseGeocodeService))]
	[InlineData(typeof(ILocationIqReverseGeocodeService))]
	public void AddressRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHostWithReverseGeocode<AddressRunner, AddressOptions>(AddressOptionsFakes.Valid(), TextWriterFakes.Valid());
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
	public void SettingsRunner_Dependencies_Resolved_Verify_Not_Null(Type type)
	{
		var host = Program.BuildHost<SettingsRunner, SettingsOptions>(SettingsOptionsFakes.Valid(), TextWriterFakes.Valid());
		var sut = host.Services.CreateScope().ServiceProvider;
		var service = sut.GetService(type);
		service.Should().NotBeNull();
	}

	#endregion
}
