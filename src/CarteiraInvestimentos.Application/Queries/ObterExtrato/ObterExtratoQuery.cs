using CarteiraInvestimentos.Application.DTOs;
using MediatR;

namespace CarteiraInvestimentos.Application.Queries.ObterExtrato;

public class ObterExtratoQuery : IRequest<List<TransacaoDto>>
{
}