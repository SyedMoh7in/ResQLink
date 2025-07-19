using System.ComponentModel.DataAnnotations;

namespace ResQLink.DTOs
{
    public class AddResponderDto
    {
        [Required]
        public string Role { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string ContactNumber { get; set; }
        public string ServiceType { get; set; }
        public string PlateNumber { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BankIBAN { get; set; }
        public string RoutingNumber { get; set; }
        public string BankSwift { get; set; }
        public string Address { get; set; }

        public IFormFile Photo { get; set; }
        public string Status { get; set; }
    }

}
