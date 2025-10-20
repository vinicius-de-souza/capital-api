namespace CarteiraInvestimentos.Application.DTOs;

public class TransacaoDto
{
    public Guid Id { get; init; }
    public string CodigoAtivo { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
    public DateTime DataOperacao { get; init; }
}