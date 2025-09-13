using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility.DtoEntity;
using Shopping.Dtos;

namespace Shopping.Models
{
    public class OrderedItem :IEntity<OrderedItemDto>
    {
        [Key]
        public int Id { get; set; }
        [Required] // <-- Zamienic na foreign key???
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public double Quantity { get; set; }
        [Required]
        public double Price { get; set; }

        public OrderedItemDto ToDto() => new OrderedItemDto
        {
            OrderId = OrderId,
            ProductId = ProductId,
            Quantity = Quantity,
            Price = Price
        };
    }
}
