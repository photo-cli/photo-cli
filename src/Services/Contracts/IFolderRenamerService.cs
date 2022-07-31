namespace PhotoCli.Services.Contracts;

public interface IFolderRenamerService
{
	void RenameByFolderAppendType(IReadOnlyCollection<Photo> orderedPhotoInfos, FolderAppendType folderAppendType, FolderAppendLocationType folderAppendLocationType,
		string targetRelativeDirectoryPath);
}
