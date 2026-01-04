using Catalog;
using Catalog.Dtos;
using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.Common;
using Utility.Enums;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : CustomControllerBase
    {
        private readonly CatalogServices _context;

        //Picture image path str class
        public class UpdateProductPicturePathRequest
        {
            public string NewPath { get; set; } = string.Empty;
        }

        public class UploadProductImageRequest
        {
            public IFormFile File { get; set; } = default!;
        }

        public ProductsController(CatalogServices context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            return await _context.GetProducts();
        }

        [HttpGet("numberOfProducts")]
        public async Task<ActionResult<int>> GetNumberOfProducts()
        {
            return await _context.GetNumberOfProducts();
        }


        [HttpGet("page/{page:int}", Name ="GetProductsPage")]
        public async Task<IEnumerable<ProductDto>> GetProducts(int page)
        {
            return await _context.GetProducts(page);
        }

        // GET: api/Products/5
        [HttpGet("{id:int}", Name = "GetProductById")]
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
            return await _context.PutProduct(id, dto, GetUserData());
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto dto)
        {
            return await _context.PostProduct(dto, GetUserData());
        }

        // DELETE: api/Products/5
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return await _context.DeleteProduct(id, GetUserData());
        }

        /*
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        */

        //------------- CRUD controllers for image path

        
        [HttpGet("prPic/{id}")]
        public async Task<ActionResult<String>> getProductPicturePath(int id)
        {
            return await _context.GetProductPicturePath(id);
        }

        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        [HttpPost("prPic/{id:int}")]
        public async Task<IActionResult> PostProductPicturePath(int id, [FromBody] UpdateProductPicturePathRequest req)
        {
            //This method is temporally "broken" and should not be used because it's deleting thumbnail path
            return await _context.PostProductPath(id, GetUserData(), req.NewPath, null);
        }

        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        [HttpDelete("prPic/{id}")]
        public async Task<IActionResult> deleteProductPicturePath(int id)
        {
            return await _context.DeleteProductPath(id, GetUserData());
        }

        //------------- CRUD operations diretcly for pictures

        [HttpGet("image/{id:int}")]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var file = await _context.GetProductImage(id);

            if (file is null)
                return NotFound();

            return PhysicalFile(file.Value.Path, file.Value.ContentType);
        }

        [HttpGet("thumbnail/{id:int}")]
        public async Task<IActionResult> GetProductThumbnail(int id)
        {
            var file = await _context.GetProductThumbnail(id);

            if (file is null)
                return NotFound();

            return PhysicalFile(file.Value.Path, file.Value.ContentType);
        }

        [HttpPost("{productId:int}/image")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        public async Task<IActionResult> UploadImage(int productId, [FromForm] UploadProductImageRequest req, CancellationToken ct)
        {
            return await _context.UploadProductImage(productId, GetUserData(), req.File, ct);
        }

    }
}

