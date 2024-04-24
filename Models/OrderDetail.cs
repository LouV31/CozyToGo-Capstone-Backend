using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CozyToGo.Models
{
    public class OrderDetail
    {
        [Key]
        public int IdOrderDetail { get; set; }
        [ForeignKey("Order")]
        public int IdOrder { get; set; }

        [ForeignKey("Dish")]
        public int IdDish { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public bool isDelivered { get; set; } = false;

        public virtual Order Order { get; set; }

        public virtual Dish Dish { get; set; }
    }
}
