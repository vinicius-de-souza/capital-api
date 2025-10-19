using CarteiraInvestimentos.Application.DTOs;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using MediatR;

namespace CarteiraInvestimentos.Application.Queries.ObterResumoCarteira;

public class ObterResumoCarteiraQueryHandler : IRequestHandler<ObterResumoCarteiraQuery, ResumoCarteiraDto>
{
    private readonly IRepositorioAtivo _repositorioAtivo;

    public ObterResumoCarteiraQueryHandler(IRepositorioAtivo repositorioAtivo)
    {
        _repositorioAtivo = repositorioAtivo;
    }

    public async Task<ResumoCarteiraDto> Handle(ObterResumoCarteiraQuery request, CancellationToken cancellationToken)
    {
        var ativos = await _repositorioAtivo.ObterTodosAsync();

        var dtos = ativos.Select(a => new AtivoDto
        {
            Codigo = a.Codigo,
            QuantidadeTotal = a.QuantidadeTotal,
            PrecoMedioCompra = a.PrecoMedioCompra,
            DataPrimeiraCompra = a.DataPrimeiraCompra,
            ValorTotalAlocado = a.QuantidadeTotal * a.PrecoMedioCompra
        }).ToList();


        var valorTotal = dtos.Sum(a => a.ValorTotalAlocado);

        var resumo = new ResumoCarteiraDto
        {
            ValorTotalCarteira = valorTotal,
            Ativos = dtos
        };

        return resumo;
    }
}