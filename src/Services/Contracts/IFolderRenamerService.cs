namespace PhotoCli.Services.Contracts;

public interface IFolderRenamerService
{
	IReadOnlyCollection<Photo> RenameByFolderAppendType(IReadOnlyCollection<Photo> orderedPhotos, FolderAppendType folderAppendType, FolderAppendLocationType folderAppendLocationType,
		string targetRelativeDirectoryPath);
}
