using FluentValidation;

namespace PhotoCli.Utils.Extensions;

public static class ValidatorExtensions
{
	public static IRuleBuilderOptions<T, string?> RequiredString<T>(this IRuleBuilder<T, string?> ruleBuilder, string? customErrorMessage = null)
	{
		return ruleBuilder.SetValidator(new RequiredStringValidator<T>(customErrorMessage));
	}
}
