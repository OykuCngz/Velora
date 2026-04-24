using MediatR;
using Microsoft.EntityFrameworkCore;
using Velora.Application.Common.Interfaces;
using Velora.Core.Entities;

namespace Velora.Application.Products.Queries;

public class GetProductDetailsHandler : IRequestHandler<GetProductDetailsQuery, Product?>
{
    private readonly IApplicationDbContext _context;

    public GetProductDetailsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
    }
}
