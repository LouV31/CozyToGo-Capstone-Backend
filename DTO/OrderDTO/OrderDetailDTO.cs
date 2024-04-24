using CozyToGo.DTO.RestaurantDTO;

namespace CozyToGo.DTO.OrderDTO
{
    public class OrderDetailDTO
    {
        public int IdOrder { get; set; }
        public int IdDish { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool isDelivered { get; set; }
        public RestaurantForOrdersDTO Restaurant { get; set; }
    }
}
