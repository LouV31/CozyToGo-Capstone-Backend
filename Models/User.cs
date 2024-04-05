using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string City { get; set; }
        [Required]
        public string Address { get; set; }
        [Display(Name = "Your secondary address")]
        public string? Address2 { get; set; }
        [Display(Name = "Your tertiary address")]
        public string? Address3 { get; set; }
        [Required]
        [Display(Name = "Zip-Code")]
        public string ZipCode { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }
        [Required]
        public string Role { get; set; } = "User";

        public virtual ICollection<Order> Orders { get; set; }
    }
}
