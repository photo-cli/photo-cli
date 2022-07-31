using FluentValidation;
using FluentValidation.Validators;

namespace PhotoCli.Utils.Validators;

public class RequiredStringValidator<T> : PropertyValidator<T, string?>
{
	private readonly string? _customErrorMessage;

	public RequiredStringValidator(string? customErrorMessage)
	{
		_customErrorMessage = customErrorMessage;
	}

	public override string Name => "RequiredStringValidator";

	public override bool IsValid(ValidationContext<T> context, string? value)
	{
		return value.IsPresent();
	}

	protected override string GetDefaultMessageTemplate(string errorCode)
	{
		return _customErrorMessage ?? "`{PropertyName}` should be a valid string";
	}
}
