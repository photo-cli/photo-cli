namespace PhotoCli.Tests.UnitTests.Services;

public class ConsoleWriterUnitTests
{
	[Fact]
	public void StandardWriteLine_Works_Without_Error()
	{
		var stringWriter = new StringWriter();
		const string expectedOutput = "test";
		new ConsoleWriter(stringWriter, NullLogger<ConsoleWriter>.Instance).Write(expectedOutput);
		var actualOutput = stringWriter.GetStringBuilder().ToString();
		actualOutput.Should().Be(expectedOutput + Environment.NewLine);
	}
}
