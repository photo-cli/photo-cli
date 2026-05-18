namespace PhotoCli.Tests.Fakes;

public record CacheEntryFake(int SampleId)
{
	public static CacheEntryFake Valid() => Sample(1);
	public static CacheEntryFake Sample(int id) => new(id);
}
