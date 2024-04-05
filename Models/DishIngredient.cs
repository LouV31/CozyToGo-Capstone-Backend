using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class DishIngredient
    {

        [Key]
        public int IdDishIngredient { get; set; }
        [ForeignKey("Dish")]
        public int IdDish { get; set; }
        [ForeignKey("Ingredient")]
        public int IdIngredient { get; set; }

        public virtual Dish Dish { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }
}
