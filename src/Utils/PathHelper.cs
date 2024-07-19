using System.Runtime.InteropServices;

namespace PhotoCli.Utils;

public static class PathHelper
{
	public static char PathSeparator()
	{
		var pathSeparator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '\\' : '/';
		return pathSeparator;
	}

	public static string AppendToTheBottomDirectory(FolderAppendLocationType folderAppendLocationType, string directoryPath, string toAppend, string folderAppendSeparator)
	{
		if (directoryPath == string.Empty)
			return toAppend;
		var separatorLastIndex = directoryPath.LastIndexOf(PathSeparator());
		var bottomDirectoryName = separatorLastIndex > -1 ? directoryPath[(separatorLastIndex + 1)..] : directoryPath;
		var upperFolderPath = separatorLastIndex > -1 ? directoryPath[..separatorLastIndex] : string.Empty;
		var lastPart = folderAppendLocationType switch
		{
			FolderAppendLocationType.Prefix => $"{toAppend}{folderAppendSeparator}{bottomDirectoryName}",
			FolderAppendLocationType.Suffix => $"{bottomDirectoryName}{folderAppendSeparator}{toAppend}",
			_ => throw new PhotoCliException($"Not implemented {nameof(FolderAppendLocationType)}: {nameof(folderAppendLocationType)}")
		};
		return Path.Combine(upperFolderPath, lastPart);
	}

	public static string TrimFolderSeparators(string value)
	{
		return value.Trim('/', '\\');
	}

	public static string FilePathWithoutExtension(string filePath)
	{
		var lastIndexOfDot = filePath.LastIndexOf('.');
		return filePath[..lastIndexOfDot];
	}

	public static (string, string) GetFileNameExtensionSeparately(string fullFilePath)
	{
		try
		{
			var separatorLastIndex = fullFilePath.LastIndexOf(PathSeparator());
			var lastIndexOfDot = fullFilePath.LastIndexOf('.');
			var fileName = fullFilePath.Substring(separatorLastIndex + 1, lastIndexOfDot - separatorLastIndex - 1);
			var extension = fullFilePath.Substring(lastIndexOfDot + 1, fullFilePath.Length - lastIndexOfDot - 1);
			return (fileName, extension);
		}
		catch (Exception ex)
		{
			throw new PhotoCliException("File path format is not correct", ex);
		}
	}

	public static string NormalizePath(string value)
	{
		return value.ToLowerInvariant();
	}
}
