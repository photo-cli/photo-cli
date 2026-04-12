namespace PhotoCli.Options;

public enum ListType : byte
{
	Summary = 0,
	Albums = 1,
	PhotosByAlbumId = 2,
	PhotosByAlbumName = 3,
	PhotosByExactDate = 4,
	PhotosByDateRange = 5,
}
