namespace PhotoCli.Models.Enums;

public enum FolderProcessType : byte
{
	Unset = 0,
	Single = 1,
	SubFoldersPreserveFolderHierarchy = 2,
	FlattenAllSubFolders = 3,
}
