using Microsoft.EntityFrameworkCore;
using Velora.Core.Entities;

namespace Velora.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
