using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Shopping;
using Shopping.Resolvers;
using Shopping.Services;
using Shopping.Services.Facade;
using System.Reflection;

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
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

/*
//Wymuszenie utworzenia resolvera - IDK czemu normalnie sie nie tworzy
//Jednak resolver siê tworzy w momencie wywo³ania. Services wywo³uje metodê z Facade -> Facade jest tworzone
//Facade wywo³uje metodê z Resolvera, resolver jest tworzony.
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
