namespace BusinessLogic.DTOs.Users
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
