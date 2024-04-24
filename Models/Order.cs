using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class Order
    {
        [Key]
        public int IdOrder { get; set; }
        [ForeignKey("User")]
        public int IdUser { get; set; }
        [Required]
        public decimal Total { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? DeliveryDate { get; set; }
        [Required]
        public string DeliveryAddress { get; set; }
        [Required]
        public string City { get; set; }

        public string? Notes { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
