namespace PhotoCli.Models;

public record AlbumEntity(string Name, AlbumType Type, DateTime CreatedAt)
{
	public int Id { get; }
	public required string Configuration { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime? ModifiedAt { get; set; }
	public ICollection<AlbumHistoryEntity>? History { get; set; }
}
