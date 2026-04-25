using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Velora.Infrastructure.Data;
using Velora.Core.Entities;

namespace Velora.Web.Controllers;

public class CartController : Controller
{
    private readonly AppDbContext _context;
    private const string CartSessionKey = "VeloraCart";

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var cart = GetCart();
        return View(cart);
    }

    public async Task<IActionResult> AddToCart(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();

        var cart = GetCart();
        var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);

        if (cartItem == null)
        {
            cart.Add(new CartItem 
            { 
                ProductId = productId, 
                ProductName = product.Name, 
                Price = product.Price, 
                Quantity = 1,
                ImageUrl = product.ImageUrl
            });
        }
        else
        {
            cartItem.Quantity++;
        }

        SaveCart(cart);
        return RedirectToAction("Index");
    }

    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Remove(item);
        }
        SaveCart(cart);
        return RedirectToAction("Index");
    }

    private List<CartItem> GetCart()
    {
        var sessionData = HttpContext.Session.GetString(CartSessionKey);
        return sessionData == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(sessionData)!;
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
    }
}

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Total => Price * Quantity;
}
