namespace PhotoCli.Models.Enums;

public enum CopyNoPhotoTakenDateAction : byte
{
	Continue = 0,
	PreventProcess = 1,
	DontCopyToOutput = 2,
	InSubFolder = 3,
	AppendToEndOrderByFileName = 4,
	InsertToBeginningOrderByFileName = 5,
}
