using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SQLiteLB.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace SQLiteLB.Models.Context
{
    public class dbTestContext : DbContext
    {
        public dbTestContext(DbContextOptions<dbTestContext> options)
            : base(options)
        {
            EnsureSeedData();
        }

        public DbSet<Product> Products { get; set; }

        public void EnsureSeedData()
        {
            // 檢查是否已經有資料
            if (!Products.Any())
            {
                // 使用 Bogus 生成假資料
                var faker = new Faker<Product>()
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(10, 1000)));

                var fakeProducts = faker.Generate(50); // 生成 50 筆假資料

                Products.AddRange(fakeProducts);
                SaveChanges(); // 寫入資料庫
            }
        }
    }
}
