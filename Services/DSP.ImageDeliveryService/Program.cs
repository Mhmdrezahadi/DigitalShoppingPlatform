using DSP.ImageDeliveryService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ImageServiceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ImageServiceConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
