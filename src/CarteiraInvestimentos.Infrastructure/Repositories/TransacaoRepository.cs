using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using CarteiraInvestimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarteiraInvestimentos.Infrastructure.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly CarteiraInvestimentosContext _context;

    public TransacaoRepository(CarteiraInvestimentosContext context)
    {
        _context = context;
    }

    public async Task<List<Transacao>> ObterTodasOrdenadasPorDataAsync()
    {
        return await _context.Transacoes
            .AsNoTracking()
            .OrderByDescending(t => t.DataOperacao)
            .ToListAsync();
    }
}