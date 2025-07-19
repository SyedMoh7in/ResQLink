using System.ComponentModel.DataAnnotations;

namespace ResQLink.DTOs
{
    public class RegisterDto
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Location { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        public string? ProfilePicture { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

    }


}
