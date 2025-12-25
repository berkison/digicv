namespace Simple.DTOs
{
    public class UserDto
    {

        public required string Password { get; set; }
        public required string Email { get; set; }
        public string? Languages { get; set; }
        public string? ReferenceName { get; set; }
        public string? ReferenceContact { get; set; }
    }
}
