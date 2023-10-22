namespace XsisMovie.Common.Dtos {
    public class UserDto {
        public string UserName { get; set; }
        public string? Password { get; set; }
    }
    public record UserRec(string username, string password);
}
