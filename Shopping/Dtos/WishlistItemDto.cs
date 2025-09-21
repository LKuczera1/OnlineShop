using Utility.DtoEntity;
using Shopping.Models;

namespace Shopping.Dtos
{
    public class WishlistItemDto :IDto<WishlistItem>
    {
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public double Quantity { get; set; }

        public WishlistItem ToEntity(int id) => new WishlistItem
        {
            Id = id,
            ProductId = ProductId,
            Quantity = Quantity,
            ClientId = ClientId
        };
    }
}

