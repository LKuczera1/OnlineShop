using Microsoft.EntityFrameworkCore;
using Shopping;
using Shopping.Services;
using Shopping.Services.Facade;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ShoppingDbContext>(
    options => options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Shopping;Trusted_Connection=True;"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Rejestracja serwisów i fasady w aplikacji
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ShoppingFacade>();

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

app.Run();
