namespace PhotoCli.Tests.Utils;

public readonly record struct ConsoleOutputValues(int PhotoFound, int PhotoCopied = 0, int HasTakenDateAndCoordinate = 0, int HasTakenDateButNoCoordinate = 0, int HasNoTakenDateAndCoordinate = 0,
	int DirectoriesCreated = 0, int CompanionsFound = 0, int CompanionsCopied = 0);
