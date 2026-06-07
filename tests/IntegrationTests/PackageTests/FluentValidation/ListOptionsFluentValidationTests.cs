namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class ListOptionsFluentValidationTests : BaseFluentValidationTests<ListOptions, ListOptionsValidator>
{
	#region Valid

	[Fact]
	public void ValidOptions_ShouldHaveNoError()
	{
		ValidationShouldHaveNoError(ListOptionsFakes.Summary());
	}

	[Fact]
	public void NullArchivePath_ShouldStoreNull()
	{
		var options = new ListOptions(listType: ListType.Summary, archivePath: null);
		options.ArchivePath.Should().BeNull();
	}

	[Fact]
	public void NullCustomDatabasePath_ShouldStoreNull()
	{
		var options = new ListOptions(listType: ListType.Summary, archivePath: "archive-path", customDatabasePath: null);
		options.CustomDatabasePath.Should().BeNull();
	}

	#endregion

	protected override ListOptionsValidator CreateValidator()
	{
		return new ListOptionsValidator();
	}
}
