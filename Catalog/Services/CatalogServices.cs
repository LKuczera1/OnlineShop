using Catalog.Dtos;
using Catalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Common;
using Utility.DtoEntity;

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
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _context.Set<Product>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            if(product == null)
            {
                return new NotFoundResult();
            }

            return product.ToDto();
        }

        //Put
        public async Task<IActionResult> PutProduct(int id, ProductDto dto, UserData userData)
        {
            switch(userData.priviledgeLevel)
            {
                case Utility.Enums.PrivilegeLevel.Admin:
                case Utility.Enums.PrivilegeLevel.SalesDepartmentWorker:

                    var entity = await _context.Set<Product>().FindAsync([id]);
                    if (entity is null)
                        return new NotFoundResult();

                    entity.FromDto(id, dto);

                    //_context.Entry(entity).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    return new NoContentResult();
                    break;
                default:
                    return new BadRequestResult();
            }
        }

        //Post
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto dto, UserData userData)
        {
            switch (userData.priviledgeLevel)
            {
                case Utility.Enums.PrivilegeLevel.Admin:
                case Utility.Enums.PrivilegeLevel.SalesDepartmentWorker:

                    var entity = dto.ToEntity(0);

                    _context.Set<Product>().Add(entity);
                    await _context.SaveChangesAsync();

                    return new CreatedAtRouteResult(nameof(GetProductById), new { id = entity.Id }, entity);
                    break;
                default:
                    return new BadRequestResult();
            }
        }

        //Delete
        public async Task<IActionResult> DeleteProduct(int id, UserData userData)
        {
            switch (userData.priviledgeLevel)
            {
                case Utility.Enums.PrivilegeLevel.Admin:
                case Utility.Enums.PrivilegeLevel.SalesDepartmentWorker:

                    var product = await _context.Products.FindAsync(id);
                    if (product == null)
                    {
                        return new NotFoundResult();
                    }

                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();

                    return new NoContentResult();
                    break;
                default:
                    return new BadRequestResult();
            }
        }
    }
}
