using System.ComponentModel.DataAnnotations;
using Utility.DtoEntity;
using Shopping.Dtos;

namespace Shopping.Models
{
    public class ShoppingCartItem :IEntity<ShoppingCartItemDto>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public double Quantity { get; set; }
        [Required]
        public double Price { get; set; }

        public ShoppingCartItemDto ToDto() => new ShoppingCartItemDto
        {
            ClientId = ClientId,
            ProductId = ProductId,
            Quantity = Quantity,
            Price = Price
        };

        public void FromDto(int id, ShoppingCartItemDto dto)
        {
            Id = id;
            ClientId = dto.ClientId;
            ProductId = dto.ProductId;
            Quantity = dto.Quantity;
            Price = dto.Price;
        }
    }
}

