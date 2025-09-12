using Catalog.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Models
{
    public class Product
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

        public ProductDto ToDto() => new ProductDto
        {
            Name = Name,
            Description = Description,
            Price = Price,
            Stock = Stock,
        };
    }
}
