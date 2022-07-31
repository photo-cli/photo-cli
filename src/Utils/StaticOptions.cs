namespace PhotoCli.Utils;

public static class StaticOptions
{
	public static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		NumberHandling = JsonNumberHandling.AllowReadingFromString
	};
}
