using Shopping.Enums;
using Utility.DtoEntity;
using Shopping.Models;

namespace Shopping.Dtos
{
    public class OrderDto :IDto<Order>
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public OrderStatus Status { get; set; }
        public double TotalPrice { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? PackedTime { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? DeliveredTime { get; set; }

        public Order ToEntity(int id) => new Order
        {
            Id = id,
            OrderId = OrderId,
            ClientId = ClientId,
            Status = Status,
            TotalPrice = TotalPrice,
            OrderTime = OrderTime,
            PackedTime = PackedTime,
            SendTime = SendTime,
            DeliveredTime = DeliveredTime,
        };

        public OrderStatusDto ToOrderStatus() => new OrderStatusDto
        {
            Status = Status,
            OrderTime = OrderTime,
            PackedTime = PackedTime,
            SendTime = SendTime,
        };
    }
}
