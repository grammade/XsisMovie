using System.ComponentModel.DataAnnotations;

namespace XsisMovie.Entities;

public class Password {
    [Key]
    public Guid id { get; set; }
    public byte[] salt { get; set; }
    public string hash { get; set; }
}
