namespace PhotoCli.Models.Enums;

public enum ExitCode
{
	Unset = -1,
	Success = 0,

	// Initializing
	ParseArgsFailed = 1,
	AppSettingsInvalidFile = 2,

	// Validation
	ApiKeyStoreValidationFailed = 10,
	AddressOptionsValidationFailed = 11,
	InfoOptionsValidationFailed = 12,
	CopyOptionsValidationFailed = 13,
	SettingsOptionsValidationFailed = 14,

	// File system
	InputFolderNotExists = 20,
	NoPhotoFoundOnDirectory = 21,
	OutputFolderIsNotEmpty = 22,
	OutputPathIsExists = 23,
	OutputPathDontHaveWriteFilePermission = 24,
	OutputPathDontHaveCreateDirectoryPermission = 25,
	InputFileNotExists = 26,
	FileVerifyErrors = 27,

	// Photo exif info
	PhotosWithNoDatePreventedProcess = 30,
	PhotosWithNoCoordinatePreventedProcess = 31,
	PhotosWithNoCoordinateAndNoDatePreventedProcess = 32,

	// Settings
	PropertyNotFound = 40,
	InvalidSettingsValue = 41,
}
