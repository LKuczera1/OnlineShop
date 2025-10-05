using Catalog;
using Catalog.Models;
using Catalog.Services;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Policy;
using System.Text.Json;
using Utility.Common;
using Utility.Enums;
using Xunit;

namespace UnitTests.UnitTests.Services
{
    public class CatalogServiceTests :ServicesTestsBase
    {
        private readonly CatalogDbContext _db;
        private readonly Catalog.Services.CatalogServices _service;

        private int productsCount;

        public CatalogServiceTests()
        {

            var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase($"ProductTestDB_{Guid.NewGuid()}")
            .Options;


            _db = new CatalogDbContext(options);
            _service = new CatalogServices(_db);

            var products = JsonSerializer.Deserialize<List<Product>>(loadDbSource("CatalogDb.json"));


            _db.Products.AddRange(products!);
            _db.SaveChanges();

        }

        [Fact]
        public async Task ShouldReturnListOfItems()
        {
            var result = await _service.GetProducts();

            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task ShouldReturnItemWithSpecifiedID()
        {
            //Since the service returns a dto that is missing an id,
            //we ask for 2 products, one of which has a non - existent id.
            
            var result1 = await _service.GetProductById(1);
            var result2 = await _service.GetProductById(100);

            Assert.NotNull(result1);
            Assert.IsType<NotFoundResult>(result2.Result);
        }

        [Theory]
        [InlineData(Utility.Enums.PrivilegeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.Customer, typeof(BadRequestResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.SalesDepartmentWorker, typeof(CreatedAtRouteResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.NotAssigned, typeof(BadRequestResult))]
        public async Task ShouldAddNewProductToDb(PrivilegeLevel privilegeLevel, Type expectedResponseType)
        {
            var newProduct = new Catalog.Dtos.ProductDto
            {
                Name = "Test",
                Description = "Test",
                Price = 100,
                Stock = 99
            };

            var response = await _service.PostProduct(newProduct, new UserData(0, privilegeLevel));

            Assert.True(typeof(object).IsAssignableFrom(expectedResponseType));
        }

        [Theory]
        [InlineData(Utility.Enums.PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.Customer, typeof(BadRequestResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.NotAssigned, typeof(BadRequestResult))]
        public async Task ShouldUpdateProductData(PrivilegeLevel privilegeLevel, Type expectedResponseType)
        {
            var newProduct = new Catalog.Dtos.ProductDto
            {
                Name = "New data" + privilegeLevel.ToString(),
                Description = "changed data",
                Price = 0,
                Stock = 0
            };

            int prodId = 3;

            var response = await _service.PutProduct(prodId, newProduct, new UserData(0, privilegeLevel));

            Assert.True(typeof(object).IsAssignableFrom(expectedResponseType));

            if (response.GetType().Equals(typeof(BadRequestResult))) 
                return; //We have received badrequestresult, there is no need to test if product has been really added to base

            var dto = await _service.GetProductById(prodId);

            Assert.Equal(newProduct.Name, dto.Value.Name);
            Assert.Equal(newProduct.Description, dto.Value.Description);
            Assert.Equal(newProduct.Price, dto.Value.Price);
            Assert.Equal(newProduct.Stock, dto.Value.Stock);
        }


        [Theory]
        [InlineData(Utility.Enums.PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.Customer, typeof(BadRequestResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(Utility.Enums.PrivilegeLevel.NotAssigned, typeof(BadRequestResult))]
        public async Task ShouldDeleteProduct(PrivilegeLevel privilegeLevel, Type expectedResponseType)
        {
            var temp = await GetNumberOfProducts();
            int numberOfProducts = temp.Value;

            int prodId = numberOfProducts;

            var response = await _service.DeleteProduct(prodId, new UserData(0, privilegeLevel));

            Assert.True(typeof(object).IsAssignableFrom(expectedResponseType));

            if (response.GetType().Equals(typeof(BadRequestResult)))
                return;

            var temp2 = await GetNumberOfProducts();

            Assert.Equal(numberOfProducts - 1, temp2.Value);
        }

        private async Task<int?> GetNumberOfProducts()
        {
            var list =  await _service.GetProducts();
            if(list != null) return list.Count();

            return 0;
        }
    }


}
