namespace PhotoCli.Services.Contracts;

public interface IExifDataGrouperService
{
	Dictionary<string, List<Photo>> Group(IEnumerable<Photo> photos, NamingStyle namingStyle);
}
