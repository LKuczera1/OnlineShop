using Catalog.Dtos;
using Catalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Services
{
    public class CatalogServices
    {
        private readonly CatalogDbContext _context;
        public CatalogServices(CatalogDbContext context)
        {
            _context = context;
        }

        //Get
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var productsList = await _context.Set<Product>().ToListAsync();

            var products = productsList.Select(p => p.ToDto());

            return products;
        }

        //Get by Id
        public async Task<ProductDto> GetProductsById(int id)
        {
            var product = await _context.Set<Product>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            return product.ToDto();
        }

        //Put
        public async Task<IActionResult> PutProduct(int id, ProductDto dto)
        {
            var entity = await _context.Set<Product>().FindAsync([id]);
            if (entity is null)
                return new NotFoundResult();

            entity = dto.ToEntity(id);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<Product>> PostProduct(ProductDto dto)
        {
            var entity = dto.ToEntity(0);

            _context.Set<Product>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(PostProduct), new { id = entity.Id }, entity);
        }

        //Post
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new NotFoundResult();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }


    }
}
