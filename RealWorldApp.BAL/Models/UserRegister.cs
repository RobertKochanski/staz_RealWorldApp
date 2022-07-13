namespace RealWorldApp.BAL.Models
{
    public class UserRegisterContainer
    {
        public UserRegister User { get; set; }
    }

    public class UserRegister
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
