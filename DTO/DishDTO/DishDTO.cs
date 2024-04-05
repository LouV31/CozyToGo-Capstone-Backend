using CozyToGo.DTO.IngredientDTO;

namespace CozyToGo.DTO.DishDTO
{
    public class DishDTO
    {
        public int IdDish { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
        public int IdRestaurant { get; set; }
        public IngredientsDTO[] Ingredients { get; set; }
    }
}
