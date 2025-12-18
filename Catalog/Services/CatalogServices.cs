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
        private readonly string _picturesPath;
        public CatalogServices(CatalogDbContext context, string picturesPath)
        {
            _context = context;
            _picturesPath = picturesPath;
        }

        //Get
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var productsList = await _context.Set<Product>().ToListAsync();

            var products = productsList.Select(p => p.ToDto());

            return products;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts(int page)
        {
            const int pageSize = 20;
            page = Math.Max(page, 1);
            

            var productsList = await _context.Set<Product>()
                                                .AsNoTracking()
                                                .OrderBy(p => p.Id)
                                                .Skip((page - 1) * pageSize)
                                                .Take(pageSize)
                                                .Select(p => p.ToDto())
                                                .ToListAsync();

            return productsList;
        }

        //Get by Id
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _context.Set<Product>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            if (product == null)
            {
                return new NotFoundResult();
            }

            return product.ToDto();
        }

        //Put
        public async Task<IActionResult> PutProduct(int id, ProductDto dto, UserData userData)
        {
            switch (userData.privilegeLevel)
            {
                case Utility.Enums.PriviledgeLevel.Admin:
                case Utility.Enums.PriviledgeLevel.SalesDepartmentWorker:

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
            switch (userData.privilegeLevel)
            {
                case Utility.Enums.PriviledgeLevel.Admin:
                case Utility.Enums.PriviledgeLevel.SalesDepartmentWorker:

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
            switch (userData.privilegeLevel)
            {
                case Utility.Enums.PriviledgeLevel.Admin:
                case Utility.Enums.PriviledgeLevel.SalesDepartmentWorker:

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

        //--------------Products pictures CRUD operations

        public async Task<ActionResult<String>> GetProductPath(int productId)
        {
            var product = await _context.Set<Product>().Where(c => c.Id.Equals(productId)).SingleOrDefaultAsync();

            if (product == null || product.PicturePath == null)
            {
                return new NotFoundResult();
            }

            return product.PicturePath;
        }

        //update
        public async Task<ActionResult> PostProductPath(int productId, UserData userData, String productPath)
        {
            switch (userData.privilegeLevel)
            {
                case Utility.Enums.PriviledgeLevel.Admin:
                case Utility.Enums.PriviledgeLevel.SalesDepartmentWorker:

                    var entity = await _context.Set<Product>().FindAsync([productId]);
                    if (entity is null)
                        return new NotFoundResult();

                    entity.PicturePath = productPath;

                    //_context.Entry(entity).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    return new NoContentResult();
                    break;
                default:
                    return new BadRequestResult();
            }
        }

        public async Task<ActionResult> DeleteProductPath(int productId, UserData userData)
        {
            return await PostProductPath(productId, userData, null);
        }

        //------------- image services methods....

        public async Task<(string Path, string ContentType)?> GetProductImage(int id)
        {
            var path = (await GetProductPath(id)).Value;

            if (path == null || !System.IO.File.Exists(path))
                return null;

            //Temp solution
            return (path, "application/octet-stream");
        }

        public async Task<ActionResult> UploadProductImage(int productId, UserData userData, IFormFile file, CancellationToken ct = default)
        {
            switch (userData.privilegeLevel)
            {
                case Utility.Enums.PriviledgeLevel.Admin:
                case Utility.Enums.PriviledgeLevel.SalesDepartmentWorker:

                    if (file == null || file.Length == 0)
                        return new BadRequestResult();

                    Directory.CreateDirectory(_picturesPath);

                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"product_{productId}_{Guid.NewGuid():N}{ext}";
                    var fullPath = Path.Combine(_picturesPath, fileName);

                    await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await file.CopyToAsync(stream, ct);
                    }

                    await PostProductPath(productId, new UserData(null, Utility.Enums.PriviledgeLevel.Admin), fullPath);

                    return new OkResult();

                default:
                    return new ForbidResult();
            }
        }

        /*
         To be honest image involved features are total mess and was quickly written just to be able
        to get product image to mobile app
        Main detected problems: 
        -When posting new pic or changing path old pic is not deleted
        -because of using "application/octet-stream" as file type end user gets file without extensions
         so we always have to assume that file extension is .jpg. AND THAT IS VERY IMPORTANT. WE HAVE TO
         KEEP THIS FORMAT WHILE UPLOADING AND DOWNLOADING. There is no problem with uploading but format is stored
         properly but we have no option (Quick option, there are always options...) to get file extension.
         or we have... While getting file path we also get file extension. And with this we can determine file extension.
         but this is temp solution...
         
         
         */
    }
}

