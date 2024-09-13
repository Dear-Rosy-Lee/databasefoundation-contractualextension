namespace YuLinTu.Component.Account.Models
{
    public class LoginInfo
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public bool Remember { get; set; }

        public LoginInfo(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}