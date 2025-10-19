using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Interfaces;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using MediatR;

namespace CarteiraInvestimentos.Application.Commands.ComprarAtivo;

public class ComprarAtivoCommandHandler : IRequestHandler<ComprarAtivoCommand, Unit>
{
    private readonly IRepositorioAtivo _repositorioAtivo;
    private readonly IUnitOfWork _unitOfWork;

    public ComprarAtivoCommandHandler(IRepositorioAtivo repositorioAtivo, IUnitOfWork unitOfWork)
    {
        _repositorioAtivo = repositorioAtivo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ComprarAtivoCommand request, CancellationToken cancellationToken)
    {
        var ativo = await _repositorioAtivo.ObterPorCodigoAsync(request.CodigoAtivo);

        if (ativo == null)
        {
            ativo = Ativo.CriarNovo(request.CodigoAtivo, request.Quantidade, request.PrecoUnitario);
            await _repositorioAtivo.AdicionarAsync(ativo);
        }
        else
        {
            ativo.Comprar(request.Quantidade, request.PrecoUnitario);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}