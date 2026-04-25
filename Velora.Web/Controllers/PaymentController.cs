using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Text.Json;

namespace Velora.Web.Controllers;

public class PaymentController : Controller
{
    private const string CartSessionKey = "VeloraCart";

    public IActionResult CreateCheckoutSession()
    {
        var cartData = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartData)) return RedirectToAction("Index", "Cart");

        var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartData);
        
        var domain = "http://localhost:5000";
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + "/Payment/Success",
            CancelUrl = domain + "/Cart/Index",
        };

        foreach (var item in cartItems!)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100), // Stripe cent cinsinden çalışır
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductName,
                    },
                },
                Quantity = item.Quantity,
            });
        }

        var service = new SessionService();
        Session session = service.Create(options);

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult Success()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return View();
    }
}
