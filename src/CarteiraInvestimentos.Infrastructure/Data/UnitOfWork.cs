using CarteiraInvestimentos.Domain.Interfaces;

namespace CarteiraInvestimentos.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly CarteiraInvestimentosContext _context;

    public UnitOfWork(CarteiraInvestimentosContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}