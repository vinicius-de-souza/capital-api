using CarteiraInvestimentos.Application.DTOs;

namespace CarteiraInvestimentos.Application.Queries.ObterResumoCarteira
{
    public class ResumoCarteiraDto
    {
        public decimal ValorTotalCarteira { get; init; }
        public List<AtivoDto> Ativos { get; init; } = new();
    }
}