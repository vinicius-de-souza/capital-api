using CarteiraInvestimentos.Domain.Exceptions;
using CarteiraInvestimentos.Domain.Interfaces;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using MediatR;

namespace CarteiraInvestimentos.Application.Commands.VenderAtivo;

public class VenderAtivoCommandHandler : IRequestHandler<VenderAtivoCommand, Unit>
{
    private readonly IRepositorioAtivo _repositorioAtivo;
    private readonly IUnitOfWork _unitOfWork;

    public VenderAtivoCommandHandler(IRepositorioAtivo repositorioAtivo, IUnitOfWork unitOfWork)
    {
        _repositorioAtivo = repositorioAtivo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(VenderAtivoCommand request, CancellationToken cancellationToken)
    {
        var ativo = await _repositorioAtivo.ObterPorCodigoAsync(request.CodigoAtivo);

        if (ativo == null)
        {
            throw new NotFoundException($"Ativo '{request.CodigoAtivo}' não encontrado na carteira.");
        }

        ativo.Vender(request.Quantidade);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}