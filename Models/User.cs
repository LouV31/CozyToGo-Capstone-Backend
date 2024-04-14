using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [NotMapped]
        public string MainAddress { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }
        public string Role { get; set; } = "User";



        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
