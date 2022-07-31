namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class SettingsOptionsFluentValidationTests : BaseFluentValidationTests<SettingsOptions, SettingsOptionsValidator>
{
	#region Valid

	[Fact]
	public void List_Flow_Without_Arguments_Should_Have_No_Error()
	{
		var options = new SettingsOptions();
		ValidationShouldHaveNoError(options);
	}

	[Fact]
	public void Get_Flow_With_Key_Option_Should_Have_No_Error()
	{
		var options = new SettingsOptions("key");
		ValidationShouldHaveNoError(options);
	}

	[Fact]
	public void Set_Flow_With_Key_And_Value_Option_Should_Have_No_Error()
	{
		var options = new SettingsOptions("key", "value");
		ValidationShouldHaveNoError(options);
	}

	[Fact]
	public void Reset_Flow_With_Reset_Option_Should_Have_No_Error()
	{
		var options = new SettingsOptions(reset: true);
		ValidationShouldHaveNoError(options);
	}

	#endregion

	#region Invalid

	[Fact]
	public void Only_Using_Value_Option_Should_Give_RequiredStringValidator_For_Key_Option_And_Verify_Error_Message()
	{
		var options = new SettingsOptions(value: "value");
		CheckPropertyRequiredString(options, nameof(SettingsOptions.Key), Required(nameof(SettingsOptions.Key), "key", 'k'));
	}

	[Fact]
	public void When_Using_Reset_Option_Using_Any_Key_Value_Option_Should_Give_NullValidator()
	{
		CheckPropertyNull(new SettingsOptions("key", reset: true), nameof(SettingsOptions.Key));
		CheckPropertyNull(new SettingsOptions(value: "value", reset: true), nameof(SettingsOptions.Value));
	}

	#endregion
}
