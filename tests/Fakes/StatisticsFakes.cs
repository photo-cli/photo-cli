namespace PhotoCli.Tests.Fakes;

public static class StatisticsFakes
{
	public static Statistics Empty()
	{
		return new Statistics();
	}

	public static Statistics Basic(int photosFound, int photosCopied = 0, int photosSame = 0, int hasTakenDateAndCoordinate = 0, int hasTakenDateButNoCoordinate = 0,
		int hasNoTakenDateAndCoordinate = 0, int directoriesCreated = 0)
	{
		return new Statistics
		{
			PhotosFound = photosFound,
			PhotosCopied = photosCopied,
			PhotosSame = photosSame,
			PhotoThatHasTakenDateAndCoordinate = hasTakenDateAndCoordinate,
			PhotoThatHasTakenDateButNoCoordinate = hasTakenDateButNoCoordinate,
			PhotoThatNoCoordinateAndNoTakenDate = hasNoTakenDateAndCoordinate,
			DirectoriesCreated = directoriesCreated,
		};
	}

	public static Statistics Companions(int photosFound, int photosCopied, int photosSame, int hasTakenDateAndCoordinate, int hasTakenDateButNoCoordinate, int hasNoTakenDateAndCoordinate, int directoriesCreated,
		int companionsFound, int companionsCopied)
	{
		var statistics = Basic(photosFound, photosCopied, photosSame, hasTakenDateAndCoordinate, hasTakenDateButNoCoordinate, hasNoTakenDateAndCoordinate, directoriesCreated);
		statistics.CompanionFilesFound = companionsFound;
		statistics.CompanionFilesCopied = companionsCopied;
		return statistics;
	}
}
