using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MovieStore.API.Models
{
    public class User 
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string EmailId { get; set; }

        public byte[] PasswordSalt { get; set; }

        public byte[] PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
