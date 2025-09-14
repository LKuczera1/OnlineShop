using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog;
using Catalog.Models;
using Catalog.Services;
using Catalog.Dtos;

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
        public async Task<IActionResult> PutProduct(int id, ProductDto dto)
        {
            return await _context.PutProduct(id, dto);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto dto)
        {
            return await _context.PostProduct(dto);
        }

        // DELETE: api/Products/5
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
