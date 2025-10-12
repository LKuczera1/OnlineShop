using Shopping.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Utility.DtoEntity;
using Shopping.Dtos;

namespace Shopping.Models
{
    public class Order : IEntity<OrderDto>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Paid;
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        public DateTime OrderTime { get; set; } = DateTime.Now;
        [AllowNull]
        public DateTime? PackedTime { get; set; }
        [AllowNull]
        public DateTime? SendTime { get; set; }
        [AllowNull]
        public DateTime? DeliveredTime { get; set; }

        public OrderDto ToDto() => new OrderDto
        {
            OrderId = OrderId,
            ClientId = ClientId,
            Status = Status,
            TotalPrice = TotalPrice,
            OrderTime = OrderTime,
            PackedTime = PackedTime,
            SendTime = SendTime,
            DeliveredTime = DeliveredTime,
        };

        public void FromDto(int id, OrderDto dto)
        {
            Id = id;
            OrderId = dto.OrderId;
            ClientId = dto.ClientId;
            Status = dto.Status;
            TotalPrice = dto.TotalPrice;
            OrderTime = dto.OrderTime;
            PackedTime = dto.PackedTime;
            SendTime = dto.SendTime;
            DeliveredTime = dto.DeliveredTime;
        }
    }
}


