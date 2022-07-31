namespace PhotoCli.Tests.Fixtures;

public class SetEnvironmentVariablesFromLaunchSettingsFixture
{
	public SetEnvironmentVariablesFromLaunchSettingsFixture()
	{
		const string launchSettingsPath = "Properties/launchSettings.json";
		using var file = File.OpenText(launchSettingsPath);
		var configuredEnvironmentVariable = new HashSet<string>();
		var launchSettingsDoc = JsonDocument.Parse(file.BaseStream);
		if (!launchSettingsDoc.RootElement.TryGetProperty("profiles", out var profilesElement))
			return;
		foreach (var profileElement in profilesElement.EnumerateObject())
		{
			if (!profileElement.Value.TryGetProperty("environmentVariables", out var environmentVariablesElement))
				continue;
			foreach (var environmentVariableProperty in environmentVariablesElement.EnumerateObject())
			{
				var name = environmentVariableProperty.Name.ToUpperInvariant();
				var isUnique = configuredEnvironmentVariable.Add(name);
				if (!isUnique)
				{
					throw new PhotoCliException(
						$"All environment variables located on {launchSettingsPath} is set. Using multiple environment variable is inconsistent with environment variable {name}");
				}

				Environment.SetEnvironmentVariable(name, environmentVariableProperty.Value.GetString());
			}
		}
	}
}
