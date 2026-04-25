using Microsoft.AspNetCore.Identity;
using Velora.Core.Entities;
using Velora.Infrastructure.Data;

namespace Velora.Infrastructure.Services;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        context.Database.EnsureCreated();

        // Rollere Ekleme
        string[] roleNames = { "Admin", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Admin Kullanıcısı Oluşturma
        var adminEmail = "admin@velora.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Velora",
                LastName = "Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        if (context.Products.Any())
        {
            context.OrderItems.RemoveRange(context.OrderItems);
            context.Orders.RemoveRange(context.Orders);
            context.Products.RemoveRange(context.Products);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();
        }

        var categories = new List<Category>
        {
            new Category { Name = "Furniture", Description = "Artisanal furniture for intentional living" },
            new Category { Name = "Lighting", Description = "Sculptural lighting objects" },
            new Category { Name = "Art & Decor", Description = "Curated objects and artwork" },
            new Category { Name = "Wellness", Description = "Essentials for mind and body" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        var products = new List<Product>
        {
            new Product { 
                Name = "Ethos Lounge Chair", 
                Description = "Crafted with a deliberate focus on tactile honesty and architectural silhouette.", 
                Price = 1240.00m, 
                StockQuantity = 10, 
                CategoryId = categories[0].Id, 
                ImageUrl = "/images/chair.png" 
            },
            new Product { 
                Name = "Horizon Table Lamp", 
                Description = "A mushroom-shaped lamp that casts a warm, ethereal glow.", 
                Price = 189.00m, 
                StockQuantity = 25, 
                CategoryId = categories[1].Id, 
                ImageUrl = "/images/lamp.png" 
            },
            new Product { 
                Name = "Serenity Art Print", 
                Description = "Minimalist abstract art in a premium light oak frame.", 
                Price = 120.00m, 
                StockQuantity = 50, 
                CategoryId = categories[2].Id, 
                ImageUrl = "/images/print.png" 
            },
            new Product { 
                Name = "Dawn Mist Candle", 
                Description = "Hand-poured soy candle with notes of bergamot and cedar.", 
                Price = 38.00m, 
                StockQuantity = 100, 
                CategoryId = categories[3].Id, 
                ImageUrl = "/images/candle.png" 
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}

