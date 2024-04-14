namespace CozyToGo.DTO.AddressDTO
{
    public class EditAddressDTO
    {
        public int IdAddress { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }


        public string AddressName { get; set; }

        public int IdUser { get; set; }
    }
}
