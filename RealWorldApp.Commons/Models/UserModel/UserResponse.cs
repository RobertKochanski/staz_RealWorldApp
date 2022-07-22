namespace RealWorldApp.Commons.Models.UserModel
{
    public class UserResponseContainer
    {
        public UserResponse User { get; set; }
    }

    public class UserResponse
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
