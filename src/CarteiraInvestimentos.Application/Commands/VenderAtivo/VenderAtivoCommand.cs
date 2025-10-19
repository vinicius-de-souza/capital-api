using MediatR;

namespace CarteiraInvestimentos.Application.Commands.VenderAtivo;

public class VenderAtivoCommand : IRequest<Unit>
{
    public string CodigoAtivo { get; set; } = default!;
    public int Quantidade { get; set; }
}