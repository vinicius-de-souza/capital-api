using CarteiraInvestimentos.Application.Commands.ComprarAtivo;
using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Interfaces;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using Moq;

namespace CarteiraInvestimentos.Application.Tests;

public class ComprarAtivoCommandHandlerTests
{
    private readonly Mock<IRepositorioAtivo> _repositorioAtivoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ComprarAtivoCommandHandler _handler;

    public ComprarAtivoCommandHandlerTests()
    {
        _repositorioAtivoMock = new Mock<IRepositorioAtivo>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new ComprarAtivoCommandHandler(
            _repositorioAtivoMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_QuandoAtivoNaoExiste_DeveCriarNovoEAdicionarAoRepositorio()
    {
        var command = new ComprarAtivoCommand
        {
            CodigoAtivo = "PETR4",
            Quantidade = 10,
            PrecoUnitario = 30
        };

        _repositorioAtivoMock.Setup(r => r.ObterPorCodigoAsync(It.IsAny<string>()))
            .ReturnsAsync((Ativo?)null);

        await _handler.Handle(command, CancellationToken.None);

        _repositorioAtivoMock.Verify(
            r => r.AdicionarAsync(It.Is<Ativo>(a => a.Codigo == command.CodigoAtivo)),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_QuandoAtivoJaExiste_DeveChamarComprarESalvar()
    {
        var command = new ComprarAtivoCommand
        {
            CodigoAtivo = "PETR4",
            Quantidade = 10,
            PrecoUnitario = 40
        };

        var ativoExistente = Ativo.CriarNovo("PETR4", 10, 30);

        _repositorioAtivoMock.Setup(r => r.ObterPorCodigoAsync(command.CodigoAtivo))
            .ReturnsAsync(ativoExistente);

        await _handler.Handle(command, CancellationToken.None);

        _repositorioAtivoMock.Verify(
            r => r.AdicionarAsync(It.IsAny<Ativo>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        Assert.Equal(20, ativoExistente.QuantidadeTotal);
    }
}