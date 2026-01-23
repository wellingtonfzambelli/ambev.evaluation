using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context) =>
        _context = context;

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Sales.FindAsync(new object?[] { id }, cancellationToken);

    public async Task<IReadOnlyList<Sale>> ListAsync(CancellationToken cancellationToken = default) =>
        await _context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync(cancellationToken);
}