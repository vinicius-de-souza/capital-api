using MediatR;

namespace CarteiraInvestimentos.Application.Commands.ComprarAtivo;

public class ComprarAtivoCommand : IRequest<Unit>
{
    public string CodigoAtivo { get; set; } = default!;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}