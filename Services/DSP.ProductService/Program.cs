using DSP.ProductService.Data;
using DSP.ProductService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IManageService, ManageService>();
builder.Services.AddScoped<ISellService, SellService>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddMemoryCache();


builder.Services.AddDbContext<ProductServiceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductServiceConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
