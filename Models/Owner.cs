using System.ComponentModel.DataAnnotations;

namespace CozyToGo.Models
{
    public class Owner
    {
        [Key]
        public int IdOwner { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; } = "Owner";

        public virtual Restaurant Restaurant { get; set; }

    }
}
