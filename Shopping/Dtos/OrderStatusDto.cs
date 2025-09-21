using Shopping.Enums;

namespace Shopping.Dtos
{
    public class OrderStatusDto
    {
        public OrderStatus Status { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? PackedTime { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? DeliveredTime { get; set; }
    }
}

