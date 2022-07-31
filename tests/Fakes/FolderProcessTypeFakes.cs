namespace PhotoCli.Tests.Fakes;

public static class FolderProcessTypeFakes
{
	public static FolderProcessType Valid()
	{
		return FolderProcessType.Single;
	}

	public static FolderProcessType OtherThanSubFoldersPreserveFolderHierarchy()
	{
		return FolderProcessType.Single;
	}

	public static FolderProcessType OtherThanSingle()
	{
		return FolderProcessType.FlattenAllSubFolders;
	}
}
