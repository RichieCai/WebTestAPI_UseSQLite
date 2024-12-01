using Microsoft.EntityFrameworkCore;
using SQLiteLB.Models.Context;
using SQLiteLB.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//添加資料庫服務 
builder.Services.AddDbContext<dbTestContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
.EnableSensitiveDataLogging()
       .LogTo(Console.WriteLine));

builder.Services.AddControllers(); builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 確保資料庫被建立
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<dbTestContext>();
     dbContext.Database.EnsureCreated();
    //dbContext.Database.Migrate();
    if (!dbContext.Products.Any())
    {
        var faker = new Bogus.Faker<Product>()
           // .RuleFor(p => p.Id, f => f.UniqueIndex + 1)
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price(10, 1000)));

        dbContext.Products.AddRange(faker.Generate(50));
        dbContext.SaveChanges();
    }
}

app.Run();
