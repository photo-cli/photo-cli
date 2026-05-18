namespace PhotoCli.Models;

public record PhotoReverseGeocodeTask(Photo Photo, Task<ReverseGeocodeAddressResult> ReverseGeocodeTask);
