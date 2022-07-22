namespace RealWorldApp.Commons.Models.UserModel
{
    public class UserUpdateModelContainer
    {
        public UserUpdateModel User { get; set; }
    }

    public class UserUpdateModel
    {
        public string UserName { get; set; }
        public string? Bio { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public string Password { get; set; }
    }
}
