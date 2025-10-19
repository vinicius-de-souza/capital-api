using CarteiraInvestimentos.Domain.Enums;
using CarteiraInvestimentos.Domain.Exceptions;

namespace CarteiraInvestimentos.Domain.Entities;

public class Ativo
{
    private readonly List<Transacao> _transacoes = new();

    public string Codigo { get; private set; } = default!;
    public int QuantidadeTotal { get; private set; }
    public decimal PrecoMedioCompra { get; private set; }
    public DateTime DataPrimeiraCompra { get; private set; }

    public IReadOnlyCollection<Transacao> Transacoes => _transacoes.AsReadOnly();

    private Ativo() { }

    public static Ativo CriarNovo(string codigo, int quantidade, decimal precoUnitario)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("O código do ativo não pode ser nulo ou vazio.", nameof(codigo));
        }

        var ativo = new Ativo
        {
            Codigo = codigo,
            DataPrimeiraCompra = DateTime.UtcNow
        };

        ativo.Comprar(quantidade, precoUnitario);
        return ativo;
    }

    public void Comprar(int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
        {
            throw new ArgumentException("A quantidade da compra deve ser maior que zero.", nameof(quantidade));
        }
        if (precoUnitario <= 0)
        {
            throw new ArgumentException("O preço unitário da compra deve ser maior que zero.", nameof(precoUnitario));
        }

        var valorTotalAtual = QuantidadeTotal * PrecoMedioCompra;
        var valorDaCompra = quantidade * precoUnitario;

        QuantidadeTotal += quantidade;
        PrecoMedioCompra = (valorTotalAtual + valorDaCompra) / QuantidadeTotal;

        _transacoes.Add(Transacao.Criar(Codigo, TipoOperacao.Compra, quantidade, precoUnitario));
    }

    public void Vender(int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new ArgumentException("A quantidade da venda deve ser maior que zero.", nameof(quantidade));
        }

        if (quantidade > QuantidadeTotal)
        {
            throw new DomainException("Saldo insuficiente para venda.");
        }

        QuantidadeTotal -= quantidade;

        _transacoes.Add(Transacao.Criar(Codigo, TipoOperacao.Venda, quantidade, PrecoMedioCompra));
    }
}