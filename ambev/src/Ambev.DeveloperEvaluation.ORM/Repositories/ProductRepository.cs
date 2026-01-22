using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    public ProductRepository(DefaultContext context) =>
        _context = context;

    // TODO: implement pagination
    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Products.ToListAsync(cancellationToken);

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Products.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
}