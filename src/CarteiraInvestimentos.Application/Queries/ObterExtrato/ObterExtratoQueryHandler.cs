using CarteiraInvestimentos.Application.DTOs;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using MediatR;

namespace CarteiraInvestimentos.Application.Queries.ObterExtrato;

public class ObterExtratoQueryHandler : IRequestHandler<ObterExtratoQuery, List<TransacaoDto>>
{
    private readonly ITransacaoRepository _transacaoRepository;

    public ObterExtratoQueryHandler(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public async Task<List<TransacaoDto>> Handle(ObterExtratoQuery request, CancellationToken cancellationToken)
    {
        var transacoes = await _transacaoRepository.ObterTodasOrdenadasPorDataAsync();

        return transacoes.Select(t => new TransacaoDto
        {
            Id = t.Id,
            CodigoAtivo = t.CodigoAtivo,
            Tipo = t.Tipo.ToString(),
            Quantidade = t.Quantidade,
            PrecoUnitario = t.PrecoUnitario,
            DataOperacao = t.DataOperacao
        }).ToList();
    }
}