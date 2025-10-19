using CarteiraInvestimentos.Domain.Entities;
using CarteiraInvestimentos.Domain.Enums;
using CarteiraInvestimentos.Domain.Exceptions;

namespace CarteiraInvestimentos.Domain.Tests;

public class AtivoTests
{
    [Fact]
    public void Vender_ComQuantidadeMaiorQueTotal_DeveLancarDomainException()
    {
        var ativo = Ativo.CriarNovo("PETR4", 10, 20.00m);
        var quantidadeParaVender = 11;

        var atoDeVender = () => ativo.Vender(quantidadeParaVender);

        var exception = Assert.Throws<DomainException>(atoDeVender);

        Assert.Equal("Saldo insuficiente para venda.", exception.Message);
    }

    [Fact]
    public void Comprar_CalculoPrecoMedio_DeveEstarCorreto()
    {
        var ativo = Ativo.CriarNovo("MGLU3", 100, 10.00m);

        ativo.Comprar(100, 20.00m);

        // Valor total = (100 * 10) + (100 * 20) = 1000 + 2000 = 3000
        // Quantidade total = 100 + 100 = 200
        // Preço Médio = 3000 / 200 = 15
        Assert.Equal(200, ativo.QuantidadeTotal);
        Assert.Equal(15.00m, ativo.PrecoMedioCompra);
    }

    [Fact]
    public void CriarNovo_ComCodigoInvalido_DeveLancarArgumentException()
    {
        string codigoInvalido = "  ";

        var act = () => Ativo.CriarNovo(codigoInvalido, 10, 1.0m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("codigo", ex.ParamName);
        Assert.Contains("O código do ativo não pode ser nulo ou vazio.", ex.Message);
    }

    [Fact]
    public void CriarNovo_ComQuantidadeInicialInvalida_DeveLancarArgumentException()
    {
        var act = () => Ativo.CriarNovo("ABCD3", 0, 10.0m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("quantidade", ex.ParamName);
        Assert.Contains("A quantidade da compra deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void CriarNovo_ComPrecoUnitarioInicialInvalido_DeveLancarArgumentException()
    {
        var act = () => Ativo.CriarNovo("ABCD3", 10, 0m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("precoUnitario", ex.ParamName);
        Assert.Contains("O preço unitário da compra deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void CriarNovo_DeveDefinirPropriedadesERegistrarPrimeiraCompra()
    {
        var antes = DateTime.UtcNow;

        var ativo = Ativo.CriarNovo("BBAS3", 5, 12.34m);
        var depois = DateTime.UtcNow;

        Assert.Equal("BBAS3", ativo.Codigo);
        Assert.Equal(5, ativo.QuantidadeTotal);
        Assert.Equal(12.34m, ativo.PrecoMedioCompra);

        Assert.InRange(ativo.DataPrimeiraCompra, antes, depois);

        Assert.Single(ativo.Transacoes);
        Transacao ultima = null!;
        foreach (var t in ativo.Transacoes) ultima = t;
        Assert.NotNull(ultima);
        Assert.Equal("BBAS3", ultima.CodigoAtivo);
        Assert.Equal(TipoOperacao.Compra, ultima.Tipo);
        Assert.Equal(5, ultima.Quantidade);
        Assert.Equal(12.34m, ultima.PrecoUnitario);
    }

    [Fact]
    public void Comprar_ComQuantidadeInvalida_DeveLancarArgumentException()
    {
        var ativo = Ativo.CriarNovo("ITUB4", 1, 10m);

        var act = () => ativo.Comprar(0, 10m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("quantidade", ex.ParamName);
        Assert.Contains("A quantidade da compra deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Comprar_ComPrecoUnitarioInvalido_DeveLancarArgumentException()
    {
        var ativo = Ativo.CriarNovo("ITSA4", 1, 10m);

        var act = () => ativo.Comprar(1, 0m);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("precoUnitario", ex.ParamName);
        Assert.Contains("O preço unitário da compra deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Comprar_DeveRegistrarTransacaoCompra()
    {
        var ativo = Ativo.CriarNovo("WEGE3", 1, 10m);

        ativo.Comprar(5, 12m);

        Assert.Equal(2, ativo.Transacoes.Count);
        Transacao ultima = null!;
        foreach (var t in ativo.Transacoes) ultima = t;
        Assert.NotNull(ultima);
        Assert.Equal("WEGE3", ultima.CodigoAtivo);
        Assert.Equal(TipoOperacao.Compra, ultima.Tipo);
        Assert.Equal(5, ultima.Quantidade);
        Assert.Equal(12m, ultima.PrecoUnitario);
    }

    [Fact]
    public void Vender_ComQuantidadeZero_DeveLancarArgumentException()
    {
        var ativo = Ativo.CriarNovo("VALE3", 10, 50m);

        var act = () => ativo.Vender(0);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("quantidade", ex.ParamName);
        Assert.Contains("A quantidade da venda deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Vender_ComQuantidadeNegativa_DeveLancarArgumentException()
    {
        var ativo = Ativo.CriarNovo("VALE3", 10, 50m);

        var act = () => ativo.Vender(-1);

        var ex = Assert.Throws<ArgumentException>(act);
        Assert.Equal("quantidade", ex.ParamName);
        Assert.Contains("A quantidade da venda deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public void Vender_DeveReduzirQuantidadeERegistrarTransacaoVenda()
    {
        var ativo = Ativo.CriarNovo("PETZ3", 10, 7.50m);

        ativo.Vender(4);

        Assert.Equal(6, ativo.QuantidadeTotal);
        Assert.Equal(7.50m, ativo.PrecoMedioCompra);

        Assert.Equal(2, ativo.Transacoes.Count);
        Transacao ultima = null!;
        foreach (var t in ativo.Transacoes) ultima = t;
        Assert.NotNull(ultima);
        Assert.Equal("PETZ3", ultima.CodigoAtivo);
        Assert.Equal(TipoOperacao.Venda, ultima.Tipo);
        Assert.Equal(4, ultima.Quantidade);
        Assert.Equal(7.50m, ultima.PrecoUnitario);
    }

    [Fact]
    public void Vender_TodaPosicao_DeveZerarQuantidadeEManterPM()
    {
        var ativo = Ativo.CriarNovo("B3SA3", 10, 9.99m);

        ativo.Vender(10);

        Assert.Equal(0, ativo.QuantidadeTotal);
        Assert.Equal(9.99m, ativo.PrecoMedioCompra);
        Assert.Equal(2, ativo.Transacoes.Count);
        Transacao ultima = null!;
        foreach (var t in ativo.Transacoes) ultima = t;
        Assert.NotNull(ultima);
        Assert.Equal(TipoOperacao.Venda, ultima.Tipo);
        Assert.Equal(10, ultima.Quantidade);
        Assert.Equal(9.99m, ultima.PrecoUnitario);
    }
}