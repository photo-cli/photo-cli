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
}
