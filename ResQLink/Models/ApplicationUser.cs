using Microsoft.AspNetCore.Identity;
namespace ResQLink.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfilePicture { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Location { get; set; }
    }
}
