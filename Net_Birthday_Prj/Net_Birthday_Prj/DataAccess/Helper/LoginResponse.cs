using BusinessLogic.DTOs.Users;


namespace DataAccess.Helper
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDTO User { get; set; } = new();
    }

}
