namespace CozyToGo.DTO.CartDTO
{
    public class CartDTO
    {

        public string Notes { get; set; }
        public string DeliveryAddress { get; set; }
        public string City { get; set; }
        public List<CartDishDTO> Dishes { get; set; }
    }
}
