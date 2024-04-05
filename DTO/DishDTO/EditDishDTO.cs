namespace CozyToGo.DTO.DishDTO
{
    public class EditDishDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int[] Ingredients { get; set; }

    }
}
