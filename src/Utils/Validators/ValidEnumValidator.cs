using FluentValidation;
using FluentValidation.Validators;

namespace PhotoCli.Utils.Validators;

public class ValidEnumValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
	public override string Name => "ValidEnumValidator";

	private readonly bool _allowUnsetDefaultValue;
	private readonly Type _type;

	public ValidEnumValidator(bool allowUnsetDefaultValue = false)
	{
		_allowUnsetDefaultValue = allowUnsetDefaultValue;
		var type = typeof(TProperty);
		_type = Nullable.GetUnderlyingType(type) ?? type;
	}

	public override bool IsValid(ValidationContext<T> context, TProperty value)
	{
		if (value == null)
			return true;

		if (!_type.IsEnum)
			return false;

		if (!_allowUnsetDefaultValue)
		{
			var numericValue = Convert.ToInt32(value);
			if (numericValue == 0)
				return false;
		}

		return Enum.IsDefined(_type, value);
	}

	protected override string GetDefaultMessageTemplate(string errorCode)
	{
		var possibleOptions = new List<string>();
		foreach (var enumValue in Enum.GetValues(_type))
		{
			var enumNumericValue = Convert.ToInt32(enumValue);
			if (enumNumericValue == 0 && !_allowUnsetDefaultValue)
				continue;
			possibleOptions.Add($"{enumValue} ({enumNumericValue})");
		}

		return $"`{{PropertyName}}` should be a valid Enum, possible options: {string.Join(", ", possibleOptions)}";
	}
}
