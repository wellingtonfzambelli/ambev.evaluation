using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class BranchRepository : IBranchRepository
{
    private readonly DefaultContext _context;

    public BranchRepository(DefaultContext context) =>
        _context = context;

    // TODO: implement pagination
    public async Task<IEnumerable<Branch>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Branches.ToListAsync(cancellationToken);

    public async Task<Branch?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.Branches.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
}
