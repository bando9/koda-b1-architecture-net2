
using KodaStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KodaStore.Infrastructure.Seeding;

public static class ProductSeedData
{
    
    private static IEnumerable<Product> InitialProducts(long defaultCategory)
    {
        yield return new Product("P001", "Keyboard Mekanik", 35000m, 20,defaultCategory);
        yield return new Product("P002", "Mouse", 20000m, 20, defaultCategory);
        yield return new Product("P003", "Monitor", 15000m, 20, defaultCategory);


        // *500 Data seed generate
        // for(int i = 1; i<=100; i++)
        // {
        //     string productCode = $"P{i:D3}";
        //     string producName = $"Product Dummy {i}";

        //     decimal price = 10000m+(i*1000m);
        //     int stock=20;

        //     string category = i%3 ==0 ? "Mie Ayam" : (i%2 ==0 ? "Bakso" : "Lainnya");

        //     yield return new Product(productCode,producName,price,stock,category);
        // }
    }

    public static void Seed(DbContext context)
    {
        var category = context.Set<Category>().FirstOrDefault();
        if (category == null)
        {
            category = new Category("Elektronik"); 
            context.Set<Category>().Add(category);
            context.SaveChanges();
        }

        foreach(var product in InitialProducts(category.Id))
        {
            
            bool exists = context.Set<Product>().Any(p=>p.ProductCode == product.ProductCode);

            if (!exists)
            {
                context.Set<Product>().Add(product);
            };

        }

        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, CancellationToken ct)
    {
        var category = await context.Set<Category>().FirstOrDefaultAsync(ct);
        if (category == null)
        {
            category = new Category("Elektronik"); 
            await context.Set<Category>().AddAsync(category, ct);
            await context.SaveChangesAsync(ct);
        }

        foreach (var product in InitialProducts(category.Id))
        {
            bool exists = await context.Set<Product>().AnyAsync(p => p.ProductCode == product.ProductCode, ct);

            if (!exists)
            {
                context.Set<Product>().Add(product);
            }
        }

        await context.SaveChangesAsync();
    }
}