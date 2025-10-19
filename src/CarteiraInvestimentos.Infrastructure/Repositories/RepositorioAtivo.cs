using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using CarteiraInvestimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarteiraInvestimentos.Infrastructure.Repositories;

public class RepositorioAtivo : IRepositorioAtivo
{
    private readonly CarteiraInvestimentosContext _context;

    public RepositorioAtivo(CarteiraInvestimentosContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Ativo ativo)
    {
        await _context.Ativos.AddAsync(ativo);
    }

    public async Task<Ativo?> ObterPorCodigoAsync(string codigo)
    {
        return await _context.Ativos
            .Include(a => a.Transacoes)
            .FirstOrDefaultAsync(a => a.Codigo == codigo);
    }

    public async Task<List<Ativo>> ObterTodosAsync()
    {
        return await _context.Ativos.ToListAsync();
    }

    public Task AtualizarAsync(Ativo ativo)
    {
        _context.Entry(ativo).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}