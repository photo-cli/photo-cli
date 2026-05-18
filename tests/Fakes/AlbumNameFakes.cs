namespace PhotoCli.Tests.Fakes;

public static class AlbumNameFakes
{
	public static string Valid()
	{
		return "Valid Album Name";
	}
	public static string Sample(int sampleId)
	{
		return $"Album - {sampleId}";
	}
}
