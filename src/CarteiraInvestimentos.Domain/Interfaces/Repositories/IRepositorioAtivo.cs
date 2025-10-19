using CarteiraInvestimentos.Domain.Entities;

namespace CarteiraInvestimentos.Domain.Interfaces.Repositories;

public interface IRepositorioAtivo
{
    Task<Ativo?> ObterPorCodigoAsync(string codigo);
    Task<List<Ativo>> ObterTodosAsync();
    Task AdicionarAsync(Ativo ativo);
    Task AtualizarAsync(Ativo ativo);
}