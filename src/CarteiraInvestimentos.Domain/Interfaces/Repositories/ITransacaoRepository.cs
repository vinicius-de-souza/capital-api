using CarteiraInvestimentos.Domain.Entities;

namespace CarteiraInvestimentos.Domain.Interfaces.Repositories;

public interface ITransacaoRepository
{
    Task<List<Transacao>> ObterTodasOrdenadasPorDataAsync();
}