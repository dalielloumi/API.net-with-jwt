using System.ComponentModel.DataAnnotations;

namespace PFA.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "username obligatoire")]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
