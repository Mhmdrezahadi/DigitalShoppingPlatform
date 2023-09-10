using DSP.ImageCropService;
using DSP.ImageCropService.Data;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
        services.AddHostedService<Worker>();
        services.AddDbContext<ImageDbContext>(options =>
            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ImageService_Db;Integrated Security=true"));
    })
    .Build();

await host.RunAsync();
