using CarteiraInvestimentos.Application.Commands.VenderAtivo;
using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Exceptions;
using CarteiraInvestimentos.Domain.Interfaces;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using Moq;

namespace CarteiraInvestimentos.Application.Tests;

public class VenderAtivoCommandHandlerTests
{
    private readonly Mock<IRepositorioAtivo> _repositorioAtivoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly VenderAtivoCommandHandler _handler;

    public VenderAtivoCommandHandlerTests()
    {
        _repositorioAtivoMock = new Mock<IRepositorioAtivo>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new VenderAtivoCommandHandler(
            _repositorioAtivoMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_QuandoVendaValida_DeveChamarVenderESalvar()
    {
        var command = new VenderAtivoCommand { CodigoAtivo = "PETR4", Quantidade = 5 };

        var ativoExistente = Ativo.CriarNovo("PETR4", 10, 30);

        _repositorioAtivoMock.Setup(r => r.ObterPorCodigoAsync(command.CodigoAtivo))
            .ReturnsAsync(ativoExistente);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(5, ativoExistente.QuantidadeTotal);
    }

    [Fact]
    public async Task Handle_QuandoAtivoNaoExiste_DeveLancarExcecao()
    {
        var command = new VenderAtivoCommand { CodigoAtivo = "INEXISTENTE", Quantidade = 5 };

        _repositorioAtivoMock.Setup(r => r.ObterPorCodigoAsync(command.CodigoAtivo))
            .ReturnsAsync((Ativo?)null);

        var action = () => _handler.Handle(command, CancellationToken.None);

        await Assert.ThrowsAsync<NotFoundException>(action);

        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}