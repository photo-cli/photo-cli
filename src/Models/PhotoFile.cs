using System.IO.Abstractions;

namespace PhotoCli.Models;

public record PhotoFile
{
	public PhotoFile(IFileInfo source)
	{
		var sourcePath = source.ToString();
		SourcePath = sourcePath ?? throw new PhotoCliException("Source path don't have any value");
		SourceFullPath = source.FullName;
		(FileName, Extension) = PathHelper.GetFileNameExtensionSeparately(sourcePath);
	}

	public void SetTarget(string targetRelativePath, string outputFolder, string? newName)
	{
		var fileName = newName ?? FileName;
		var normalizedExtension = PathHelper.NormalizePath(Extension);
		TargetRelativePath = Path.Combine(targetRelativePath, $"{fileName}.{normalizedExtension}");
		TargetFullPath = Path.Combine(outputFolder, TargetRelativePath);
	}

	public string SourcePath { get; private set; }
	public string SourceFullPath { get; private set; }
	public string? TargetFullPath { get; private set; }
	public string? TargetRelativePath { get; private set; }
	public string FileName { get; }
	public string Extension { get; }
	public string? Sha1Hash { get; set; }
}
