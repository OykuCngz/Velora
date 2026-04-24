using MediatR;

namespace Velora.Application.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    int CategoryId,
    string ImageUrl
) : IRequest<int>;
