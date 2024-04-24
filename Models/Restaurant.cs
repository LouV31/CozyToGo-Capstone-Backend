using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class Restaurant
    {
        [Key]
        public int IdRestaurant { get; set; }
        [ForeignKey("Owner")]
        public int IdOwner { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [Display(Name = "Zip-Code")]
        public string ZipCode { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Opening Hour")]
        public TimeSpan OpeningHours { get; set; }
        [Required]
        [Display(Name = "Closing Hour")]
        public TimeSpan ClosingHours { get; set; }
        [Required]
        [Display(Name = "Day off")]
        public string ClosingDay { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Is it still an active Restaurant?")]
        public bool IsActive { get; set; } = true;
        [Required]
        public string Image { get; set; } = "default.jpg"; // Default image 


        public virtual ICollection<Dish> Dishes { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual Owner Owner { get; set; }
    }
}
