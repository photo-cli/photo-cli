using FluentValidation;

namespace PhotoCli.Options.Validators;

public class ListOptionsValidator : BaseValidator<ListOptions>
{
	public ListOptionsValidator()
	{
		RuleFor(r => r.ListType).ValidEnum(true);

		When(w => w.ListType is ListType.PhotosByAlbumId, () =>
		{
			RuleFor(r => r.AlbumId).NotNull().WithMessage(MustUseMessage(nameof(ListOptions.AlbumId), nameof(ListType.PhotosByAlbumId)));
		});

		When(w => w.ListType is ListType.PhotosByAlbumName, () =>
		{
			RuleFor(r => r.AlbumName).NotNull().WithMessage(MustUseMessage(nameof(ListOptions.AlbumName), nameof(ListType.PhotosByAlbumName)));
		});

		When(w => w.ListType is ListType.PhotosByDateRange, () =>
		{
			RuleFor(r => r).Must(r => r.StartDate.HasValue || r.EndDate.HasValue)
				.WithMessage(MustProvideAtLeastOneOfWhen(nameof(ListType.PhotosByDateRange), nameof(ListOptions.StartDate), nameof(ListOptions.EndDate)));
		});
	}
}
