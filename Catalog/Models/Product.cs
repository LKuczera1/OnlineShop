using Catalog.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Utility.DtoEntity;

namespace Catalog.Models
{
    public class Product : IEntity<ProductDto>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double Stock { get; set; }
        
        public string? PicturePath { get; set; }

        public ProductDto ToDto() => new ProductDto
        {
            Name = Name,
            Description = Description,
            Price = Price,
            Stock = Stock,
            PicturePath = PicturePath
        };

        public void FromDto(int id, ProductDto dto)
        {
            Id = id;
            Name = dto.Name;
            Description = dto.Description;
            Price = dto.Price;
            Stock = dto.Stock;
            PicturePath = dto.PicturePath;
        }
    }
}


