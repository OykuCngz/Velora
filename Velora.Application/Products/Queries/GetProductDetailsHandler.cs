using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Velora.Application.Common.Interfaces;
using Velora.Core.Entities;

namespace Velora.Application.Products.Queries;

public class GetProductDetailsHandler : IRequestHandler<GetProductDetailsQuery, ProductDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;

    public GetProductDetailsHandler(IApplicationDbContext context, IMapper mapper, IDistributedCache cache)
    {
        _context = context;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductDto?> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"product_{request.Id}";
        var cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            return JsonSerializer.Deserialize<ProductDto>(cachedProduct);
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null) return null;

        var productDto = _mapper.Map<ProductDto>(product);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(productDto), cacheOptions, cancellationToken);

        return productDto;
    }
}
