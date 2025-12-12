using System.Text.Encodings.Web;

namespace PhotoCli.Utils;

public static class StaticOptions
{
	public static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		NumberHandling = JsonNumberHandling.AllowReadingFromString
	};

	public static readonly JsonSerializerOptions AlbumConfigurationJsonSerializerOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};

	public static readonly JsonSerializerOptions AnsiConsoleLoggerOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};
}
