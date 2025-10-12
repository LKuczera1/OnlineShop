using Utility.DtoEntity;
using Shopping.Models;

namespace Shopping.Dtos
{
    public class OrderedItemDto : IDto<OrderedItem>
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }

        public OrderedItem ToEntity(int id) => new OrderedItem
        {
            Id = id,
            OrderId = OrderId,
            ProductId = ProductId,
            Quantity = Quantity,
            Price = Price
        };
    }
}


