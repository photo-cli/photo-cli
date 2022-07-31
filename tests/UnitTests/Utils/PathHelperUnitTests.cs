namespace PhotoCli.Tests.UnitTests.Utils;

public class PathHelperUnitTests
{
	public static TheoryData<FolderAppendLocationType, string, string, string> Prefix = new()
	{
		{ FolderAppendLocationType.Prefix, MockFileSystemHelper.RelativePath("main1", "sub2"), "prefix1", MockFileSystemHelper.RelativePath("main1", "prefix1-sub2") },
		{ FolderAppendLocationType.Prefix, MockFileSystemHelper.RelativePath("main1", "sub2", "sub3"), "prefix1", MockFileSystemHelper.RelativePath("main1", "sub2", "prefix1-sub3") },
		{ FolderAppendLocationType.Prefix, "", "prefix1", "prefix1" },
		{ FolderAppendLocationType.Prefix, "path-with-no-sub", "prefix1", "prefix1-path-with-no-sub" },
	};

	public static TheoryData<FolderAppendLocationType, string, string, string> Suffix = new()
	{
		{ FolderAppendLocationType.Suffix, MockFileSystemHelper.RelativePath("main1", "sub2"), "suffix1", MockFileSystemHelper.RelativePath("main1", "sub2-suffix1") },
		{ FolderAppendLocationType.Suffix, MockFileSystemHelper.RelativePath("main1", "sub2", "sub3"), "suffix1", MockFileSystemHelper.RelativePath("main1", "sub2", "sub3-suffix1") },
		{ FolderAppendLocationType.Suffix, "", "suffix1", "suffix1" },
		{ FolderAppendLocationType.Suffix, "path-with-no-sub", "suffix1", "path-with-no-sub-suffix1" },
	};

	[Theory]
	[MemberData(nameof(Prefix))]
	[MemberData(nameof(Suffix))]
	public void AddedPrefix_Should_Be_Expected(FolderAppendLocationType folderAppendLocationType, string directoryPath, string toAppend, string expectedPath)
	{
		var value = PathHelper.AppendToTheBottomDirectory(folderAppendLocationType, MockFileSystemHelper.RelativePath(directoryPath), MockFileSystemHelper.RelativePath(toAppend), "-");
		value.Should().Be(MockFileSystemHelper.RelativePath(expectedPath));
	}
}
