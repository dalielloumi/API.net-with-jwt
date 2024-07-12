using System.ComponentModel.DataAnnotations;

namespace PFA.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Key]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? phoneNumber { get; set; }
    }

}
