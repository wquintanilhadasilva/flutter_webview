
namespace sso.Models
{
    public class UserProfile
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string OIdProvider { get; set; }
        public string OId { get; set; }
        public string Role { get; set; }
    }
}
