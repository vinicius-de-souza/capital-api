using CarteiraInvestimentos.Domain.Enums;

namespace CarteiraInvestimentos.Domain.Entities;

public class Transacao
{
    public Guid Id { get; private set; }
    public string CodigoAtivo { get; private set; } = default!;
    public TipoOperacao Tipo { get; private set; }
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public DateTime DataOperacao { get; private set; }

    private Transacao() { }

    public static Transacao Criar(string codigoAtivo, TipoOperacao tipo, int quantidade, decimal precoUnitario)
    {
        // VALIDAÇÕES 

        if (string.IsNullOrWhiteSpace(codigoAtivo))
        {
            throw new ArgumentException("O código do ativo não pode ser nulo ou vazio.", nameof(codigoAtivo));
        }

        if (quantidade <= 0)
        {
            throw new ArgumentException("A quantidade da operação deve ser maior que zero.", nameof(quantidade));
        }

        if (precoUnitario <= 0)
        {
            throw new ArgumentException("O preço unitário do ativo deve ser maior que zero.", nameof(precoUnitario));
        }

        return new Transacao
        {
            CodigoAtivo = codigoAtivo,
            Tipo = tipo,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario,
            DataOperacao = DateTime.UtcNow
        };
    }
}