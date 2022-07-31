namespace PhotoCli.Models.Enums;

public enum CopyNoCoordinateAction : byte
{
	Continue = 0,
	PreventProcess = 1,
	DontCopyToOutput = 2,
	InSubFolder = 3,
}
