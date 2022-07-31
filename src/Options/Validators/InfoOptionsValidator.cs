using FluentValidation;

namespace PhotoCli.Options.Validators;

public class InfoOptionsValidator : BaseValidator<InfoOptions>
{
	public InfoOptionsValidator()
	{
		RuleFor(r => r.OutputPath).NotNull().WithMessage(Required(nameof(CopyOptions.OutputPath), OptionNames.OutputPathOptionNameLong, OptionNames.OutputPathOptionNameShort))
			.Matches(Constants.CsvExtensionRegex).WithMessage($"{nameof(CopyOptions.OutputPath)} should have .csv extension");

		Include(new SharedReverseGeocodeValidator());
	}
}
