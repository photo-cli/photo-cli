namespace PhotoCli.Tests.Fakes.Options;

public static class ToolOptionsFakes
{
	public static ToolOptions Valid()
	{
		return ToolOptions.Default();
	}

	public static ToolOptions WithConnectionLimit(int connectionLimit)
	{
		var options = ToolOptions.Default();
		options.ConnectionLimit = connectionLimit;
		return options;
	}
}
