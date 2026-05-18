using FluentValidation;

namespace PhotoCli.Utils.Extensions;

public static class ValidatorExtensions
{
	public static IRuleBuilderOptions<T, string?> RequiredString<T>(this IRuleBuilder<T, string?> ruleBuilder, string? customErrorMessage = null)
	{
		return ruleBuilder.SetValidator(new RequiredStringValidator<T>(customErrorMessage));
	}

	public static IRuleBuilderOptions<T, TProperty> ValidEnum<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool allowUnsetDefaultValue = false)
	{
		return ruleBuilder.SetValidator(new ValidEnumValidator<T, TProperty>(allowUnsetDefaultValue));
	}

	public static IRuleBuilderOptionsConditions<T, TProperty> CustomAddFailureErrorMessage<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string errorMessage)
	{
		return ruleBuilder.Custom((_, context) => context.AddFailure(errorMessage));
	}
}
