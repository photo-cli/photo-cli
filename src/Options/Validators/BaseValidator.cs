using FluentValidation;

namespace PhotoCli.Options.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
	protected string CantUseMessage(string wantedToUse, string when)
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

	protected static string CantFindMessage(ReverseGeocodeProvider reverseGeocodeProvider, string environmentVariableKey, string longOptionName, char shortOptionName)
	{
		return $"Can't find {reverseGeocodeProvider} API key at environment variable with key {environmentVariableKey} or application arguments -{longOptionName} or -{shortOptionName}";
	}
}
