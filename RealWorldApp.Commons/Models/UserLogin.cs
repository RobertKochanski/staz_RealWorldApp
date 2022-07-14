namespace RealWorldApp.Commons.Models
{
    public class UserLoginContainer
    {
        public UserLogin User { get; set; }
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
