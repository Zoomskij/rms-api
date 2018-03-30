using System.ComponentModel.DataAnnotations;

namespace RMSAutoAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Region { get; set; }
    }
}