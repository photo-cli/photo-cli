using System.Linq.Expressions;
using CommandLine;
using FluentValidation;

namespace PhotoCli.Options.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
	protected void AddFailureToCustomState(string errorMessage)
	{
		RuleFor(r => r).CustomAddFailureErrorMessage(errorMessage);
	}

	protected string CantUseMessage(string wantedToUse, string when)
	{
		return $"Can't use {wantedToUse} when using {when}";
	}

	protected string MustUseMessage(string shouldUse, string when)
	{
		return $"Must use {shouldUse} when using {when}";
	}

	protected static string CantFindMessage(ReverseGeocodeProvider reverseGeocodeProvider, string environmentVariableKey, string longOptionName, char shortOptionName)
	{
		return $"Can't find {reverseGeocodeProvider} API key at environment variable with key {environmentVariableKey} or application arguments -{longOptionName} or -{shortOptionName}";
	}

	protected string MustAlsoUseOnlyOneOfTheOptions(params string[] options)
	{
		return $"Must use only one of the options {string.Join(", ", options)}";
	}

	protected string MustAlsoUseOnlyOneOfTheOptionsWhen(string when, params string[] options)
	{
		return $"Must use only one of the options {string.Join(", ", options)} when using {when}";
	}

	protected static string GetOptionFormatWithValue(Expression<Func<T, object?>> expression, string value)
	{
		return OptionFormatWithValue(GetOptionFormat(expression), value);
	}

	protected static string GetOptionFormatByTypeWithValue(Type actualOption, string propertyName, string value)
	{
		return OptionFormatWithValue(GetOptionFormatByType(actualOption, propertyName), value);
	}

	protected static string GetOptionFormatByType(Type actualOption, string propertyName)
	{
		var propertyInfo = actualOption.GetProperty(propertyName);
		if (propertyInfo == null)
			throw new ArgumentException($"Property {propertyName} was not found in type {actualOption}");
		var optionAttributes = propertyInfo.GetCustomAttributes<OptionAttribute>().ToList();
		if (optionAttributes.Count != 1)
			throw new ArgumentException("not found exactly one OptionAttribute");
		return OptionFormat(optionAttributes[0], propertyInfo);
	}

	protected static string GetOptionFormat(Expression<Func<T, object?>> expression)
	{
		PropertyInfo propertyInfo;
		switch (expression.Body)
		{
			case UnaryExpression unaryExpression:
				{
					if (unaryExpression.Operand is not MemberExpression memberExpression)
						throw new ArgumentException("Invalid property expression");
					var pi = memberExpression.Member as PropertyInfo;
					if (pi == null)
						throw new ArgumentException("Invalid property expression");
					propertyInfo = pi;
					break;
				}
			case MemberExpression memberExpression:
				{
					if (memberExpression.Member is not PropertyInfo pi)
						throw new ArgumentException("Invalid property expression");
					propertyInfo = pi;
					break;
				}
			default:
				throw new ArgumentException("Invalid property expression");
		}

		var optionAttributes = propertyInfo.GetCustomAttributes<OptionAttribute>().ToList();
		if (optionAttributes.Count != 1)
			throw new ArgumentException("not found exactly one OptionAttribute");
		return OptionFormat(optionAttributes[0], propertyInfo);
	}

	private static string OptionFormat(OptionAttribute optionAttribute, PropertyInfo propertyInfo)
	{
		return $"{propertyInfo.Name} ( --{optionAttribute.LongName} or -{optionAttribute.ShortName} )";
	}

	private static string OptionFormatWithValue(string optionInfo, string value)
	{
		return $"{optionInfo} with value {value}";
	}
}
