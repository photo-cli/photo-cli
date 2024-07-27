namespace PhotoCli.Utils.Extensions;

public static class ReadOnlyCollectionExtensions
{
	public static void ThrowIfNotOrderedByPhotoTakenDate(this IReadOnlyCollection<Photo> list)
	{
		var orderedPhotosThatHavePhotoTakenDate = list.Where(w => w.HasTakenDateTime).ToList();

		var validateSorted = orderedPhotosThatHavePhotoTakenDate
			.Zip(orderedPhotosThatHavePhotoTakenDate.Skip(1), (curr, next) => curr.TakenDateTime <= next.TakenDateTime)
			.All(x => x);

		if (!validateSorted)
			throw new PhotoCliException($"{nameof(list)} is not sorted by PhotoTakenDate");
	}
}
