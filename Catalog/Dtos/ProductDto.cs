using Catalog.Models;
using Utility.DtoEntity;

namespace Catalog.Dtos
{
    public class ProductDto : IDto<Product>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Stock { get; set; }
        public string? PictureName { get; set; } = string.Empty;
        public string? ThumbnailName { get; set; } = string.Empty;

        public Product ToEntity(int id) => new Product
        {
            Id = id,
            Name = Name,
            Description = Description,
            Price = Price,
            Stock = Stock,
            PictureName = PictureName,
            ThumbnailName = ThumbnailName
        };
    }
}


