using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class Dish
    {
        [Key]
        public int IdDish { get; set; }
        [ForeignKey("Restaurant")]
        public int IdRestaurant { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public string Image { get; set; } = "default.jpg";

        [Required]
        [Display(Name = "Is this item still Avaiable?")]
        public bool IsAvailable { get; set; } = true;
        [NotMapped]
        public decimal Price { get; set; }

        public virtual ICollection<DishIngredient> DishIngredients { get; set; }

        public virtual Restaurant Restaurant { get; set; }

    }
}
