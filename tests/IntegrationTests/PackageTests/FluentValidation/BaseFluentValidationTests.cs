using FluentValidation;

namespace PhotoCli.Tests.IntegrationTests.PackageTests.FluentValidation;

public abstract partial class BaseFluentValidationTests<TValue, TValidator> where TValidator : AbstractValidator<TValue>
{
	protected const string ReverseGeocodeProviderInfo = "ReverseGeocodeProvider ( --reverse-geocode or -e )";
	protected const string BigDataCloudAdminLevelInfo = "BigDataCloudAdminLevels ( --bigdatacloud-levels or -u )";
	protected const string ReverseGeocodeWithBigDataCloudInfo = "ReverseGeocodeProvider ( --reverse-geocode or -e ) with value BigDataCloud";
	protected const string OpenStreetMapPropertiesInfo = "OpenStreetMapProperties ( --openstreetmap-properties or -r )";
	protected const string GoogleMapsAddressTypeInfo = "GoogleMapsAddressTypes ( --googlemaps-types or -m )";
	protected const string ReverseGeocodeProviderWithGoogleMapsInfo = "ReverseGeocodeProvider ( --reverse-geocode or -e ) with value GoogleMaps";
	protected const string MissingReverseGeocodeActionInfo = "MissingReverseGeocodeAction ( --missing-reverse-geocode or -z )";

	protected abstract TValidator CreateValidator();

	protected void ValidationShouldHaveNoError(TValue value)
	{
		var validator = CreateValidator();
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

	protected void CheckPropertyNotEmpty(TValue value, string propertyName, string? errorMessage = null)
	{
		ErrorProperty("NotEmptyValidator", value, propertyName, errorMessage);
	}

	protected void CheckPropertyRegularExpression(TValue value, string propertyName, string? errorMessage = null)
	{
		ErrorProperty("RegularExpressionValidator", value, propertyName, errorMessage);
	}

	private void ErrorProperty(string errorCode, TValue value, string propertyName, string? errorMessageExpected = null)
	{
		var validator = CreateValidator();
		var validationResult = validator.Validate(value);
		validationResult.Errors.Count.Should().BeGreaterThan(0);
		var propertyValidationFailure = validationResult.Errors.SingleOrDefault(s => s.PropertyName == propertyName && s.ErrorCode == errorCode);
		var propertiesWhichHaveError = validationResult.Errors.Select(s => s.PropertyName);
		propertyValidationFailure.Should().NotBeNull($"No error found on {propertyName}. This properties has errors: {string.Join(", ", propertiesWhichHaveError)}");
		propertyValidationFailure?.ErrorCode.Should().Be(errorCode);
		if (errorMessageExpected != null)
			propertyValidationFailure?.ErrorMessage.Should().Be(errorMessageExpected);
	}

	protected static string CantUseMessage(string wantedToUse, string when)
	{
		return $"Can't use {wantedToUse} when using {when}";
	}

	protected string MustUseMessage(string shouldUse, string when)
	{
		return $"Must use {shouldUse} when using {when}";
	}

	protected string Required(string property, string longOptionName, char shortOptionName)
	{
		return $"{property} ( --{longOptionName} or -{shortOptionName} ) is required";
	}

	private string RequiredStringErrorMessage(string property)
	{
		var pascalCaseWithSpace = Regex.Replace(property, ".([A-Z])", m => m.Value.Insert(1, " "));
		return $"`{pascalCaseWithSpace}` is required";
	}

	protected static string ReverseGeocodeProviderInfoWithValue(ReverseGeocodeProvider reverseGeocodeProvider)
	{
		return $"ReverseGeocodeProvider ( --reverse-geocode or -e ) with value {reverseGeocodeProvider.ToString()}";
	}


	protected void CheckEnumInvalidRangeValue<TEnum>(TValue value, string propertyName, bool discardUnsetDefaultValue) where TEnum : struct, Enum
	{
		var replacePascalCasePropertyWithSingleSpace = ReplacePascalCasePropertyWithSingleSpace(propertyName);
		var errorMessage = ValidEnumMessage<TEnum>(replacePascalCasePropertyWithSingleSpace, discardUnsetDefaultValue);
		ErrorProperty("ValidEnumValidator", value, propertyName, errorMessage);
	}

	private static string ValidEnumMessage<TEnum>(string enumName, bool discardUnsetDefaultValue) where TEnum : struct, Enum
	{
		var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
		if (discardUnsetDefaultValue)
			values = values.Where(w => (byte)(object)w > 0);
		var formatted = values.Select(s => $"{s} ({(byte)(object)s})");
		var validOptionsKeyValueText = string.Join(", ", formatted);
		return $"`{enumName}` should be a valid Enum, possible options: {validOptionsKeyValueText}";
	}

	private static string ReplacePascalCasePropertyWithSingleSpace(string input)
	{
		return PascalCaseMatchingRegex().Replace(input, " $1");
	}

	[GeneratedRegex("(\\B[A-Z])")]
	private static partial Regex PascalCaseMatchingRegex();
}
