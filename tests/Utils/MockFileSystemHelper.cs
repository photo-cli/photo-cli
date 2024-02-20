using System.Runtime.InteropServices;

namespace PhotoCli.Tests.Utils;

public static class MockFileSystemHelper
{
	public static string Path(params string[] paths)
	{
		return Path(System.IO.Path.Combine(paths));
	}

	public static string Path(bool useRelativePath = false, params string[] paths)
	{
		return Path(System.IO.Path.Combine(paths), useRelativePath);
	}

	public static string Path(string path, bool useRelativePath = false)
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			return path;
		path = path.Replace("/", "\\");
		if (useRelativePath || path.StartsWith("C:"))
			return path;
		if (!path.StartsWith('\\'))
			path = '\\' + path;
		path = "C:" + path;
		return path;
	}

	public static string RelativePath(params string[] value)
	{
		return string.Join(PathHelper.PathSeparator(), value);
	}
}
