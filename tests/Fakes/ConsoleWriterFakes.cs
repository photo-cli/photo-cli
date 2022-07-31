namespace PhotoCli.Tests.Fakes;

public static class ConsoleWriterFakes
{
	public static ConsoleWriter Valid()
	{
		return new ConsoleWriter(new StringWriter(), NullLogger<ConsoleWriter>.Instance);
	}
}
