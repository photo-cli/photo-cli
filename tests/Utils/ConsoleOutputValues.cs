namespace PhotoCli.Tests.Utils;

public readonly record struct ConsoleOutputValues(int Found, int Copied = 0, int HasTakenDateAndCoordinate = 0, int HasTakenDateButNoCoordinate = 0, int HasNoTakenDateAndCoordinate = 0,
	int DirectoriesCreated = 0);
