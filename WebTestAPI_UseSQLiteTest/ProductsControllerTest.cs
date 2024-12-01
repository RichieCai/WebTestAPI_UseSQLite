using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SQLiteLB.Models.Context;
using SQLiteLB.Models.Entities;
using WebTestAPI_UseSQLite.Controllers;

namespace WebTestAPI_UseSQLiteTest
{
    public class ProductsControllerTests
    {
        private readonly dbTestContext _context;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            // 使用 SQLite 的 In-Memory Database
            var options = new DbContextOptionsBuilder<dbTestContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new dbTestContext(options);
            _context.Database.OpenConnection(); // 開啟 SQLite In-Memory 資料庫
            _context.Database.EnsureCreated();  // 確保資料庫結構已建立

            _controller = new ProductsController(_context);

            // 初始化測試數據
            _context.Products.AddRange(new List<Product>
            {
                new Product { Id = 1, Name = "Product1" },
                new Product { Id = 2, Name = "Product2" }
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
        }

        [Fact]
        public async Task GetProduct_ReturnsProduct_WhenProductExists()
        {
            var result = await _controller.GetProduct(1);

            var okResult = Assert.IsType<ActionResult<Product>>(result);
            Assert.Equal(1, okResult.Value.Id);
            Assert.Equal("Product1", okResult.Value.Name);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var result = await _controller.GetProduct(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostProduct_AddsProductAndReturnsCreatedAtAction()
        {
            var newProduct = new Product { Id = 3, Name = "Product3" };

            var result = await _controller.PostProduct(newProduct);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);

            var productInDb = _context.Products.Find(3);
            Assert.NotNull(productInDb);
            Assert.Equal("Product3", productInDb.Name);
        }

        [Fact]
        public async Task PutProduct_UpdatesProductAndReturnsNoContent()
        {
            // 清除追蹤的實體，避免衝突
            _context.ChangeTracker.Clear();
            var updatedProduct = new Product { Id = 1, Name = "UpdatedProduct1", Price = 6600 };

            var result = await _controller.PutProduct(1, updatedProduct);

            Assert.IsType<NoContentResult>(result);

            var productInDb = _context.Products.Find(1);
            Assert.NotNull(productInDb);
            Assert.Equal("UpdatedProduct1", productInDb.Name);
        }

        [Fact]
        public async Task PutProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            var updatedProduct = new Product { Id = 2, Name = "UpdatedProduct2" };

            var result = await _controller.PutProduct(1, updatedProduct);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_RemovesProductAndReturnsNoContent()
        {
            var result = await _controller.DeleteProduct(1);

            Assert.IsType<NoContentResult>(result);

            var productInDb = _context.Products.Find(1);
            Assert.Null(productInDb);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            var result = await _controller.DeleteProduct(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}