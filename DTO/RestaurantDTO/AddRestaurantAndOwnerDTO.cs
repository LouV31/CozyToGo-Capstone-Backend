using CozyToGo.DTO.OwnerDTO;

namespace CozyToGo.DTO.RestaurantDTO
{
    public class AddRestaurantAndOwnerDTO
    {
        public AddRestaurantDTO NewRestaurant { get; set; }
        public AddOwnerDTO NewOwner { get; set; }
    }
}
