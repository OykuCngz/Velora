using Microsoft.AspNetCore.Mvc;
using MediatR;
using Velora.Application.Products.Queries;

namespace Velora.Web.Controllers;

public class ProductsController : Controller
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _sender.Send(new GetProductDetailsQuery(id));

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}

