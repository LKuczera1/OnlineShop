using Catalog;
using Catalog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Note: To enable listening on ip addres not just only localport insert 0.0.0.0:port
//instead of: localhost:port

// Add services to the container.
builder.Services.AddDbContext<CatalogDbContext>(
    options => options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Catalog;Trusted_Connection=True;"));


var imagesPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Catalog","productsImages")
);
//Temporaly solution
// imagesPath => D:\Programming\Projects\Visual Studio\OnlineShop\Catalog\productsImages

if(imagesPath==null) 
    imagesPath = "D:\\Programming\\Projects\\Visual Studio\\OnlineShop\\Catalog\\productsImages"; 

//Wstrzykiwanie serwisów wraz z przekazaniem sciezki
builder.Services.AddScoped<CatalogServices>(sp =>
{
    var db = sp.GetRequiredService<CatalogDbContext>();
    return new CatalogServices(db, imagesPath);
});

builder.Services.AddControllers();
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

//Rejestracja pliku json
builder.Configuration.AddJsonFile(
    Utility.Common.Tools.GetPhysicalFileProviderToUtility(),
    "AppSettings/appsettings.json",
    optional: false,
    reloadOnChange: true);

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
builder.Services.AddAuthentication();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Comment HttpsRedirect to connect from other devices
//app.Urls.Add("http://0.0.0.0:7001"); <- also uncomment this and set the same URL in launchSettings
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


