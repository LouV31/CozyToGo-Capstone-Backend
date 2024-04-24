using CozyToGo.DTO.RestaurantDTO;

namespace CozyToGo.DTO.OrderDTO
{

    public class OrderDTO
    {
        public int IdOrder { get; set; }
        public int IdUser { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }
        public string? Notes { get; set; }
        public List<RestaurantForOrdersDTO> Restaurants { get; set; }
        //public string PaymentIntentId { get; set; }
    }

}
