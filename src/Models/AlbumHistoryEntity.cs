namespace PhotoCli.Models;

public record AlbumHistoryEntity(string Configuration, DateTime SnapshotAt)
{
	public long Id { get; }
	public virtual AlbumEntity? Album { get; set; }
}
