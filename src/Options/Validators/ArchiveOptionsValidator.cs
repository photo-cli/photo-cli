using FluentValidation;

namespace PhotoCli.Options.Validators;

public class ArchiveOptionsValidator : BaseValidator<ArchiveOptions>
{
	public ArchiveOptionsValidator()
	{
		var outputPathInfo = GetOptionFormat(e => e.OutputPath);
		var albumNameNewInfo = GetOptionFormat(e => e.AlbumNameNew);
		var albumTypeInfo = GetOptionFormat(e => e.AlbumType);
		var albumIdUpdateInfo = GetOptionFormat(e => e.AlbumIdUpdate);
		var autoAddressAlbumInfo = GetOptionFormat(e => e.AutoReverseGeocodeAlbum);
		var reverseGeocodeProviderInfo = GetOptionFormat(e => e.ReverseGeocodeProvider);

		var optionType = typeof(ArchiveOptions);
		Include(new SharedReverseGeocodeValidator(optionType));
		Include(new ActionableReverseGeocodeValidator(optionType));

		RuleFor(r => r.OutputPath).RequiredString(outputPathInfo);
		RuleFor(r => r.InvalidFileFormatAction).ValidEnum(true);
		RuleFor(r => r.NoPhotoTakenDateAction).ValidEnum(true);
		RuleFor(r => r.NoCoordinateAction).ValidEnum(true);
		RuleFor(r => r.AlbumType).ValidEnum();

		When(w => w.AlbumNameNew is not null, () =>
		{
			RuleFor(r => r.AlbumType).NotNull().WithMessage(MustUseMessage(albumTypeInfo, albumNameNewInfo));
		});

		When(w => w.AlbumIdUpdate is not null, () =>
		{
			RuleFor(r => r.AlbumType).NotNull().WithMessage(MustUseMessage(albumTypeInfo, albumIdUpdateInfo));
		});

		When(w => w.AlbumType is not null && w.AlbumNameNew is null && w.AlbumIdUpdate is null, () =>
		{
			AddFailureToCustomState(MustAlsoUseOnlyOneOfTheOptionsWhen(albumTypeInfo, albumNameNewInfo, albumIdUpdateInfo));
		});

		When(w => w.AlbumNameNew is not null && w.AlbumIdUpdate is not null, () =>
		{
			AddFailureToCustomState(MustAlsoUseOnlyOneOfTheOptions(albumNameNewInfo, albumIdUpdateInfo));
		});

		When(w => w.AutoReverseGeocodeAlbum, () =>
		{
			RuleFor(r => r.ReverseGeocodeProvider).Must(m => m != ReverseGeocodeProvider.Disabled).WithMessage(MustUseMessage(reverseGeocodeProviderInfo, autoAddressAlbumInfo));
		});
	}
}
