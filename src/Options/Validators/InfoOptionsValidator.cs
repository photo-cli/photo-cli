using FluentValidation;

namespace PhotoCli.Options.Validators;

public class InfoOptionsValidator : BaseValidator<InfoOptions>
{
	public InfoOptionsValidator()
	{
		var optionType = typeof(InfoOptions);
		Include(new SharedReverseGeocodeValidator(optionType));
		Include(new ActionableReverseGeocodeValidator(optionType));

		var outputPathInfo = GetOptionFormat(e => e.OutputPath);

		RuleFor(r => r.OutputPath)
			.RequiredString(outputPathInfo)
			.Matches(Constants.CsvExtensionRegex).WithMessage($"{nameof(CopyOptions.OutputPath)} should have .csv extension");

		RuleFor(r => r.InvalidFileFormatAction).ValidEnum(true);
		RuleFor(r => r.NoPhotoTakenDateAction).ValidEnum(true);
		RuleFor(r => r.NoCoordinateAction).ValidEnum(true);
	}
}
