namespace YuLinTu.Component.Account.Models
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Realname { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Card { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Jb { get; set; }
        public string Region { get; set; }
        public string RegionName { get; set; }
        public object Roles { get; set; }
        public object RoleIds { get; set; }
        public object Permissions { get; set; }
    }
}