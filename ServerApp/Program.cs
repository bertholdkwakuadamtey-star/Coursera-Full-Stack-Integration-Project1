using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5216")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register memory cache service
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseCors("AllowBlazorClient");

// Inject IMemoryCache into endpoint
app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    // Try to get cached products
    if (!cache.TryGetValue("products", out Product[] products))
    {
        // If not cached, build product list
        products = new[]
        {
            new Product(1, "Laptop", 1200.50, 25, new Category(101, "Electronics")),
            new Product(2, "Headphones", 50.00, 100, new Category(102, "Accessories")),
            new Product(3, "Smartphone", 799.99, 50, new Category(101, "Electronics")),
            new Product(4, "Keyboard", 30.00, 200, new Category(102, "Accessories"))
        };

        // Cache for 5 minutes
        cache.Set("products", products, TimeSpan.FromMinutes(5));
    }

    return products;
});

app.Run();

// Models
public record Category(int Id, string Name);
public record Product(int Id, string Name, double Price, int Stock, Category Category);
