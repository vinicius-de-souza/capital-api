using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Enums;

namespace CarteiraInvestimentos.Domain.Tests;

public class TransacaoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DevePreencherPropriedadesEData()
    {
        var antes = DateTime.UtcNow;

        var transacao = Transacao.Criar("ABEV3", TipoOperacao.Compra, 100, 14.25m);
        var depois = DateTime.UtcNow;

        Assert.Equal("ABEV3", transacao.CodigoAtivo);
        Assert.Equal(TipoOperacao.Compra, transacao.Tipo);
        Assert.Equal(100, transacao.Quantidade);
        Assert.Equal(14.25m, transacao.PrecoUnitario);
        Assert.InRange(transacao.DataOperacao, antes, depois);
    }

    [Fact]
    public void Criar_ComCodigoInvalido_DeveLancarArgumentException()
    {
        var act = () => Transacao.Criar("  ", TipoOperacao.Venda, 1, 1.0m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("codigoAtivo", ex.ParamName);
        Assert.Contains("O código do ativo não pode ser nulo ou vazio.", ex.Message);
    }

    [Fact]
    public void Criar_ComQuantidadeInvalida_DeveLancarArgumentException()
    {
        var act = () => Transacao.Criar("ABEV3", TipoOperacao.Compra, 0, 1.0m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("quantidade", ex.ParamName);
        Assert.Contains("A quantidade da operação deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Criar_ComPrecoUnitarioInvalido_DeveLancarArgumentException()
    {
        var act = () => Transacao.Criar("ABEV3", TipoOperacao.Venda, 1, 0m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("precoUnitario", ex.ParamName);
        Assert.Contains("O preço unitário do ativo deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Criar_ComTipoVenda_DeveManterTipo()
    {
        var transacao = Transacao.Criar("VALE3", TipoOperacao.Venda, 5, 70m);

        Assert.Equal(TipoOperacao.Venda, transacao.Tipo);
    }
}