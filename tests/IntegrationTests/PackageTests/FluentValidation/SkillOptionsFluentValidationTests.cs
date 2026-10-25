namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public class SkillOptionsFluentValidationTests : BaseFluentValidationTests<SkillOptions, SkillOptionsValidator>
{
	#region Valid

	[Fact]
	public void Without_Arguments_Should_Have_No_Error()
	{
		ValidationShouldHaveNoError(new SkillOptions());
	}

	[Fact]
	public void Global_Option_Should_Have_No_Error()
	{
		ValidationShouldHaveNoError(new SkillOptions(true));
	}

	#endregion

	protected override SkillOptionsValidator CreateValidator()
	{
		return new SkillOptionsValidator();
	}
}
