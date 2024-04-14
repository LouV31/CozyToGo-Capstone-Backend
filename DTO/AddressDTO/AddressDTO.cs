namespace CozyToGo.DTO.AddressDTO
{
    public class AddressDTO
    {
        public int IdAddress { get; set; }
        public int IdUser { get; set; }
        public string AddressName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string CompleteAddress { get; set; }
        public string ZipCode { get; set; }
        public bool IsPrimary { get; set; }
    }
}
