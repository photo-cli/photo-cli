namespace PhotoCli.Tests.Fakes.Options;

public static class SettingsOptionsFakes
{
	public static SettingsOptions Valid()
	{
		return new SettingsOptions();
	}

	public static SettingsOptions Set()
	{
		return new SettingsOptions(nameof(ToolOptionsRaw.AddressSeparator), "value");
	}
}
