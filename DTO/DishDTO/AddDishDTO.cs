namespace CozyToGo.DTO.DishDTO
{
    public class AddDishDTO
    {

        public int IdRestaurant { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }



        public List<int> Ingredients { get; set; }
    }
}
