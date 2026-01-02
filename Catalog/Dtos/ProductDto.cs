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
        public string? PicturePath { get; set; }

        public Product ToEntity(int id) => new Product
        {
            Id = id,
            Name = Name,
            Description = Description,
            Price = Price,
            Stock = Stock,
            PicturePath = PicturePath
        };
    }
}


