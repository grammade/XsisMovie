using System.ComponentModel.DataAnnotations;

namespace XsisMovie.Entities;

public class Movie : BaseEntity {
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public float? Rating { get; set; }
    public string? Image { get; set; }
}
