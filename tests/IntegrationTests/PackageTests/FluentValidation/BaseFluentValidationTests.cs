using FluentValidation;

namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public abstract class BaseFluentValidationTests<TValue, TValidator> where TValidator : AbstractValidator<TValue>, new()
{
	protected void ValidationShouldHaveNoError(TValue value)
	{
		var validator = new TValidator();
		var validationResult = validator.Validate(value);
		validationResult.IsValid.Should().BeTrue(validationResult.FlattenToSingleMessage());
	}

	protected void CheckPropertyRequiredString(TValue value, string propertyName, string? customErrorMessage = null)
	{
		var errorMessage = customErrorMessage ?? RequiredStringErrorMessage(propertyName);
		ErrorProperty("RequiredStringValidator", value, propertyName, errorMessage);
	}

	protected void CheckPropertyNotNull(TValue value, string propertyName, string? errorMessage = null)
	{
		ErrorProperty("NotNullValidator", value, propertyName, errorMessage);
	}

	protected void CheckPropertyNull(TValue value, string propertyName, string? errorMessage = null)
	{
		ErrorProperty("NullValidator", value, propertyName, errorMessage);
	}

	protected void CheckPropertyInvalidValue(TValue value, string propertyName, string? errorMessage = null)
	{
		ErrorProperty("PredicateValidator", value, propertyName, errorMessage);
	}

	protected void CheckPropertyInvalidEnumValue(TValue value, string propertyName)
	{
		ErrorProperty("EnumValidator", value, propertyName);
	}

	protected void CheckPropertyNotEmpty(TValue value, string propertyName, string? errorMessage = null)
	{
		ErrorProperty("NotEmptyValidator", value, propertyName, errorMessage);
	}

	protected void CheckPropertyRegularExpression(TValue value, string propertyName, string? errorMessage = null)
	{
		ErrorProperty("RegularExpressionValidator", value, propertyName, errorMessage);
	}

	private void ErrorProperty(string errorCode, TValue value, string propertyName, string? errorMessage = null)
	{
		var validator = new TValidator();
		var validationResult = validator.Validate(value);
		validationResult.Errors.Count.Should().BeGreaterThan(0);
		var propertyValidationFailure = validationResult.Errors.SingleOrDefault(s => s.PropertyName == propertyName && s.ErrorCode == errorCode);
		var propertiesWhichHaveError = validationResult.Errors.Select(s => s.PropertyName);
		propertyValidationFailure.Should().NotBeNull($"No error found on {propertyName}. This properties has errors: {string.Join(", ", propertiesWhichHaveError)}");
		propertyValidationFailure?.ErrorCode.Should().Be(errorCode);
		if (errorMessage != null)
			propertyValidationFailure?.ErrorMessage.Should().Be(errorMessage);
	}

	protected static string CantUseMessage(string wantedToUse, string when)
	{
		return $"Can't use {wantedToUse} when using {when}";
	}

	protected string MustUseMessage(string shouldUse, string when, string longOptionName, char shortOptionName)
	{
		return $"Must use {shouldUse} when using {when} with ( --{longOptionName} or -{shortOptionName} )";
	}

	protected string Required(string property, string longOptionName, char shortOptionName)
	{
		return $"{property} is required. ( --{longOptionName} or -{shortOptionName} )";
	}

	private string RequiredStringErrorMessage(string property)
	{
		var pascalCaseWithSpace = Regex.Replace(property, ".([A-Z])", m => m.Value.Insert(1, " "));
		return $"`{pascalCaseWithSpace}` should be a valid string";
	}
}
