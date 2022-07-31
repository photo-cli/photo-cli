namespace PhotoCli.Utils.Extensions;

public static class ReadOnlyCollectionExtensions
{
	public static void ThrowIfNotOrderedByPhotoTakenDate(this IReadOnlyCollection<Photo> list)
	{
		var orderedPhotoInfosThatHavePhotoTakenDate = list.Where(w => w.HasPhotoTakenDateTime).ToList();

		var validateSorted = orderedPhotoInfosThatHavePhotoTakenDate
			.Zip(orderedPhotoInfosThatHavePhotoTakenDate.Skip(1), (curr, next) => curr.PhotoTakenDateTime!.Value <= next.PhotoTakenDateTime!.Value)
			.All(x => x);

		if (!validateSorted)
			throw new PhotoCliException($"{nameof(list)} is not sorted by PhotoTakenDate");
	}
}
