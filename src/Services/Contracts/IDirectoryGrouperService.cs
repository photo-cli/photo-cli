namespace PhotoCli.Services.Contracts;

public interface IDirectoryGrouperService
{
	Dictionary<string, List<Photo>> GroupFiles(Dictionary<string, ExifData> photoExifDataByFilePath, string sourceRootPath, FolderProcessType folderProcessType,
		GroupByFolderType? groupByFolderType, bool noPhotoDateTimeTakenGroupedInSubFolder, bool noReverseGeocodeGroupedInSubFolder);
}
