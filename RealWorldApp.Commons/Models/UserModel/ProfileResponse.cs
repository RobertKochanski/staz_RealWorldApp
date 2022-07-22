namespace RealWorldApp.Commons.Models.UserModel
{
    public class ProfileResponseContainer
    {
        public ProfileResponse Profile { get; set; }
    }

    public class ProfileResponse
    {
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public bool following { get; set; } = false;
    }
}
