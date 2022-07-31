namespace PhotoCli.Tests.Fakes;

public static class TextWriterFakes
{
	public static TextWriter Valid()
	{
		return new StringWriter();
	}
}
