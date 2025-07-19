using Microsoft.AspNetCore.Identity;

namespace ResQLink.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Bio { get; set; }
        public string Role { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string PhoneNumber { get; set; }

        public string? ServiceType { get; set; }
        public string? PlateNumber { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankIBAN { get; set; }
        public string? RoutingNumber { get; set; }
        public string? BankSwift { get; set; }
        public string? Address { get; set; }

        public ApplicationUser User { get; set; }
    }

}
