namespace PhotoCli.Services.Contracts;

public interface IDirectoryGrouperService
{
	Dictionary<string, IReadOnlyCollection<Photo>> GroupFiles(IReadOnlyCollection<Photo> photos, string sourceRootPath, FolderProcessType folderProcessType,
		GroupByFolderType? groupByFolderType, bool invalidFileFormatGroupedInSubFolder, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder);
}
