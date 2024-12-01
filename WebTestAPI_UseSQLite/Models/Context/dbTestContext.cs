using Bogus;
using Microsoft.EntityFrameworkCore;
using SQLiteLB.Models.Entities;

namespace SQLiteLB.Models.Context
{
    public class dbTestContext : DbContext
    {
        public dbTestContext(DbContextOptions<dbTestContext> options)
            : base(options)
        {
          //  EnsureSeedData();
        }

        public DbSet<Product> Products { get; set; }

        //public void EnsureSeedData()
        //{
        //    // 檢查是否已經有資料
        //    if (!Products.Any())
        //    {
        //        // 使用 Bogus 生成假資料
        //        var faker = new Faker<Product>()
        //            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        //            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(10, 1000)));

        //        var fakeProducts = faker.Generate(50); // 生成 50 筆假資料

        //        Products.AddRange(fakeProducts);
        //        SaveChanges(); // 寫入資料庫
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //var faker = new Bogus.Faker<Product>()
            //    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            //    .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(10, 1000)));

            //modelBuilder.Entity<Product>().HasData(faker.Generate(50));
        }
    }
}
