using Utility.DtoEntity;
using Shopping.Models;

namespace Shopping.Dtos
{
    public class ShoppingCartItemDto : IDto<ShoppingCartItem>
    {
        public int? Id { get; set; } = null;
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }

        public ShoppingCartItem ToEntity(int id) => new ShoppingCartItem
        {
            Id = id,
            ClientId = ClientId,
            ProductId = ProductId,
            Quantity = Quantity,
            Price = Price
        };

        public OrderedItemDto ToOrderedItemDto(int orderId) => new OrderedItemDto()
        {

            OrderId = orderId,
            ProductId = ProductId,
            Quantity = Quantity,
            Price = Price
        };
    }
}


