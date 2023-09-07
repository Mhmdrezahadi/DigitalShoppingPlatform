using DSP.Gateway.Configs;
using DSP.Gateway.Data;
using DSP.Gateway.Entities;
using DSP.Gateway.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
                .AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions
                    .Converters.Add(new JsonStringEnumConverter());
                });
builder.Services.AddMvc();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddMemoryCache();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Digital Shopping Platform",
        Description = "Digital Shopping Platform Documentation Using Swagger",
    });

    c.OperationFilter<CategorizeFilter>();

    c.OrderActionsBy((apiDesc) =>
    {
        return $"{apiDesc.RelativePath.Split('/')[3]}_{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}_{apiDesc.RelativePath}";
    }
   );

    c.DocInclusionPredicate((name, api) => true);

    var securityScheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Description =
        @"JWT Authorization header using the Bearer scheme." +
        "\r\n\r\n" +
        "Enter TOKEN in the text input below." +
        "\r\n\r\n" +
        "Example: 'a1.b2.c3'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        BearerFormat = "JWT",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
                {
                    {securityScheme, new[] { "Bearer" } }
                };

    c.AddSecurityRequirement(securityRequirement);

    c.UseInlineDefinitionsForEnums();
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GatewayConnection"));
});
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<UserDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 1;

    // Default SignIn settings.
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Default User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.TokenValidationParameters =
    new TokenValidationParameters
    {
        LifetimeValidator = (before, expires, token, param) =>
        {
            return expires > DateTime.UtcNow;
        },
        // bayad validator algorithm ha ro piade konam
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateActor = false,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Tokens:Issuer"],
        ValidAudience = builder.Configuration["Tokens:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Tokens:Key"]))
    };

    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;

            if
            (
            !string.IsNullOrEmpty(accessToken)
            &&
            path.StartsWithSegments("/hub")
            )
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddHttpClient<CategoryHttpService>();
builder.Services.AddHttpClient<ColorHttpService>();
builder.Services.AddHttpClient<ManageHttpService>();
builder.Services.AddHttpClient<OrderHttpService>();
builder.Services.AddHttpClient<PaymentHttpService>();
builder.Services.AddHttpClient<ProductHttpService>();
builder.Services.AddHttpClient<SellHttpService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

var service = app.Services.CreateScope();

var context = service.ServiceProvider.GetRequiredService<UserDbContext>();
var userManager = service.ServiceProvider.GetRequiredService<UserManager<User>>();
var roleManager = service.ServiceProvider.GetRequiredService<RoleManager<Role>>();

try
{
    await Seeder.SeedUsers(userManager, roleManager);

    await Seeder.SeedProvince(context);

    await Seeder.SeedCities(context);
}
catch (Exception exception)
{
    //Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", exception);
    throw;
}

app.Run();
