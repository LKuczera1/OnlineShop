using Catalog;
using Catalog.Dtos;
using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.Enums;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CatalogServices _context;

        public ProductsController(CatalogServices context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            return await _context.GetProducts();
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            return await _context.GetProductById(id);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        public async Task<IActionResult> PutProduct(int id, ProductDto dto)
        {
            return await _context.PutProduct(id, dto);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto dto)
        {
            return await _context.PostProduct(dto);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return await _context.DeleteProduct(id);
        }

        /*
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        */
    }
}
