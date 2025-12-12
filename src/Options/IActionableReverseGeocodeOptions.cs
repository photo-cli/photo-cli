namespace PhotoCli.Options;

public interface IActionableReverseGeocodeOptions : IReverseGeocodeOptions
{
	MissingReverseGeocodeAction MissingReverseGeocodeAction { get; }
}
