namespace PhotoCli.Services.Contracts;

public interface IDuplicatePhotoRemoveService
{
	IReadOnlyCollection<Photo> GroupAndFilterByPhotoHash(IReadOnlyCollection<Photo> photos);
}
