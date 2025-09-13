using System.ComponentModel.DataAnnotations;
using Utility.DtoEntity;
using Shopping.Dtos;

namespace Shopping.Models
{
    public class WishlistItem :IEntity<WishlistItemDto>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public double Quantity { get; set; }

        public WishlistItemDto ToDto() => new WishlistItemDto
        {
            ProductId = ProductId,
            Quantity = Quantity,
            ClientId = ClientId
        };
    }
}
