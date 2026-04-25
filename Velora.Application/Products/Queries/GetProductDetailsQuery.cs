using MediatR;
using Velora.Core.Entities;

namespace Velora.Application.Products.Queries;

public record GetProductDetailsQuery(int Id) : IRequest<ProductDto?>;
