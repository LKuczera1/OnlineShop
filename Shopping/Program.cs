using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Shopping;
using Shopping.Resolvers;
using Shopping.Services;
using Shopping.Services.Facade;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ShoppingDbContext>(
    options => options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Shopping;Trusted_Connection=True;"));

//Rejestracja pliku json
builder.Configuration.AddJsonFile(
    new PhysicalFileProvider(AppContext.BaseDirectory),
    "AppSettings/appsettings.json",
    optional: false,
    reloadOnChange: true);

var baseUrl = builder.Configuration["Services:Catalog"];

//Rejestracja CatalogResolver

builder.Services.AddHttpClient<CatalogResolver>(client =>
    client.BaseAddress = new Uri(baseUrl)
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Wpisz: Bearer {token JWT}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//Rejestracja serwisów i fasady w aplikacji
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ShoppingFacade>();

//Ignorowanie controllerów z innych rest API
builder.Services.AddControllers()
    .ConfigureApplicationPartManager(apm =>
    {
        apm.ApplicationParts.Clear();
        apm.ApplicationParts.Add(new AssemblyPart(Assembly.GetExecutingAssembly()));
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

/*
//Wymuszenie utworzenia resolvera
//Facade wywo³uje metodê z Resolvera, resolver jest tworzony.
//Innymi s³owy, builder tworzy wszystko "On Demand"
app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    _ = scope.ServiceProvider.GetRequiredService<CatalogResolver>();
});
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
