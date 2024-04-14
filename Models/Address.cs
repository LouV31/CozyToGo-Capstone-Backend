using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class Address
    {

        [Key]
        public int IdAddress { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        [Required]
        [Display(Name = "Place name")]
        public string AddressName { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public bool IsPrimary { get; set; }
        public string CompleteAddress
        {
            get
            {
                return $"{StreetAddress}, {City}, {ZipCode}";
            }
        }

        public virtual User User { get; set; }

    }
}
