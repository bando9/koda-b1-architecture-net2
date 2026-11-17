using KodaStore.Infrastructure.Persistence;
using Scalar.AspNetCore;
using KodaStore.Api.Files;
using KodaStore.Api.Middlewares;
using KodaStore.Application.Repositories;
using KodaStore.Application.Services;
using Microsoft.EntityFrameworkCore;
using KodaStore.Infrastructure.Seeding;
using KodaStore.Infrastructure.Repositories;
using KodaStore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails(
    option =>
    {
        option.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path.Value;

            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        };
    }
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// builder.Services.AddSingleton<InMemoryProductCatalog>();

// builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
// builder.Services.AddSingleton<IProductService, ProductService>();

// Repository
// builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
// builder.Services.AddScoped<IProductRepository, EfProductRepository>();

// builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
// builder.Services.AddScoped<IProductQueryRepository, EfProductQueryRepository>();

// // Service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductImageStorage, LocalProductImageStorage>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


builder.Services.AddInfrastructure(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("ecommercedb") 
    ?? throw new InvalidOperationException("ga bisa koneksi, belum konfigurasi");


// Membaca daftar origin di env.Development
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

// Daftarkan Configuration CORS
builder.Services.AddCors(option =>
{
    option.AddPolicy("ReactClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
        .UseSeeding((context, _) =>
        {
            ProductSeedData.Seed(context);
             OrderSeedData.Seed(context);
        })
        .UseAsyncSeeding(async (context,_,ct) =>
        {
            await ProductSeedData.SeedAsync(context,ct);
            await OrderSeedData.SeedAsync(context,ct);
        });

}); 


var app = builder.Build();
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference("/api");

    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Koda Store API v1");
        options.RoutePrefix = "swagger";

        options.DocumentTitle = "Koda Store | API Documentation";

        options.EnableFilter();
    });
}
// *apply cors ReactClient policy
app.UseCors("ReactClient");

app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();