namespace PhotoCli.Services.Contracts;

public interface ISequentialNumberEnumeratorService
{
	IEnumerable<string> NumberIterator(int toNumerateCount, NumberNamingTextStyle numberNamingTextStyle);
}
