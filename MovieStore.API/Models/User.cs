using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace MovieStore.API.Models
{
    public class User : LoginModel
    {
        public int Id { get; set; }

        [Required]
        public string EmailId { get; set; }

       // [Required] 
        public byte[] PasswordSalt { get; set; }

       // [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class LoginModel
    {
        [Required]
        //[Column(TypeName = "varchar")]
        public string Username { get; set; }

        [IgnoreDataMember]
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(100)]
        public string Password { get; set; }
    }
}
