using System.ComponentModel.DataAnnotations;

namespace Orders.Models
{
    public class Order
    {
        public enum OrderStatus
        {
            Pending = 0,
            OrderPaid = 1,
            InRealisation = 2,
            SendToCustomer = 3,
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public int Customer {  get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}
