using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class Ingredient
    {
        [Key]
        public int IdIngredient { get; set; }
        [ForeignKey("Restaurant")]
        public int IdRestaurant { get; set; }
        [Required]
        [Display(Name = "Ingredient's name")]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }

        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<DishIngredient> DishIngredients { get; set; }
    }
}
