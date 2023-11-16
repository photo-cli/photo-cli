using FluentValidation;

namespace PhotoCli.Options.Validators;

public class ArchiveOptionsValidator : BaseValidator<ArchiveOptions>
{
	public ArchiveOptionsValidator()
	{
		Include(new SharedReverseGeocodeValidator());

		RuleFor(r => r.OutputPath).NotNull().WithMessage(Required(nameof(CopyOptions.OutputPath),
			OptionNames.OutputPathOptionNameLong, OptionNames.OutputPathOptionNameShort));

		RuleFor(r => r.NoPhotoTakenDateAction).IsInEnum();
		RuleFor(r => r.NoCoordinateAction).IsInEnum();
	}
}
