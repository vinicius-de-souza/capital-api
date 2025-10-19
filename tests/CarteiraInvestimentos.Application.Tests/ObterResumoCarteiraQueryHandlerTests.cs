using CarteiraInvestimentos.Application.Queries.ObterResumoCarteira;
using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using Moq;

namespace CarteiraInvestimentos.Application.Tests;

public class ObterResumoCarteiraQueryHandlerTests
{
    private readonly Mock<IRepositorioAtivo> _repositorioAtivoMock;
    private readonly ObterResumoCarteiraQueryHandler _handler;

    public ObterResumoCarteiraQueryHandlerTests()
    {
        _repositorioAtivoMock = new Mock<IRepositorioAtivo>();
        _handler = new ObterResumoCarteiraQueryHandler(_repositorioAtivoMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoExistemAtivos_DeveRetornarResumoMapeadoCorretamente()
    {
        var query = new ObterResumoCarteiraQuery();

        var listaAtivos = new List<Ativo>
        {
            Ativo.CriarNovo("PETR4", 10, 30),
            Ativo.CriarNovo("MGLU3", 100, 5)
        };

        _repositorioAtivoMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(listaAtivos);

        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, resultado.Count);

        var dtoPetr4 = resultado.First(a => a.Codigo == "PETR4");
        Assert.Equal(10, dtoPetr4.QuantidadeTotal);
        Assert.Equal(30, dtoPetr4.PrecoMedioCompra);
        Assert.Equal(300, dtoPetr4.ValorTotalAlocado);

        var dtoMglu3 = resultado.First(a => a.Codigo == "MGLU3");
        Assert.Equal(500, dtoMglu3.ValorTotalAlocado);
    }
}