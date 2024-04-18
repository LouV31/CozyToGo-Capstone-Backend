using CozyToGo.DTO.OrderDTO;

namespace CozyToGo.DTO.RestaurantDTO
{
    public class RestaurantForOrdersDTO
    {
        public int IdRestaurant { get; set; }
        public string Name { get; set; }
        public List<OrderDetailDTO> Dishes { get; set; }
    }
}
