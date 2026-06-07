namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class McpOptionsFluentValidationTests : BaseFluentValidationTests<McpOptions, McpOptionsValidator>
{
	#region Valid

	[Fact]
	public void ValidOptions_ShouldHaveNoError()
	{
		ValidationShouldHaveNoError(new McpOptions());
	}

	[Fact]
	public void NullArchivePath_ShouldStoreNull()
	{
		var options = new McpOptions(archivePath: null);
		options.ArchivePath.Should().BeNull();
	}

	[Fact]
	public void NullCustomDatabasePath_ShouldStoreNull()
	{
		var options = new McpOptions(archivePath: "archive-path", customDatabasePath: null);
		options.CustomDatabasePath.Should().BeNull();
	}

	#endregion

	protected override McpOptionsValidator CreateValidator()
	{
		return new McpOptionsValidator();
	}
}
