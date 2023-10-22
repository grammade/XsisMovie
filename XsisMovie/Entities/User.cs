using System.ComponentModel.DataAnnotations;

namespace XsisMovie.Entities;

public class User : BaseEntity {
    [Key]
    public int Id { get; set; }
    public required string UserName { get; set; }
    public string? Role { get; set; }
    public Guid PasswordId { get; set; }
    public Password? Password { get; set; }
}
