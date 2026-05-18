namespace PhotoCli.Models;

public enum AlbumResult : byte
{
	Successful = 0,
	NoPhotosToAddInAlbum = 1,
	AlbumExists = 2,
	DataInconsistency = 3,
	AlbumNotFound = 4,
	ExistingConfigurationNotInCorrectFormat = 5,
}
