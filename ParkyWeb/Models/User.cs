using System.ComponentModel.DataAnnotations;
namespace ParkyWeb.Models
{
    public class User
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
