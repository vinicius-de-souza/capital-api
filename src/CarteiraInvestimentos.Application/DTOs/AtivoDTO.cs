namespace CarteiraInvestimentos.Application.DTOs;

public class AtivoDto
{
    public required string Codigo { get; init; }
    public int QuantidadeTotal { get; init; }
    public decimal PrecoMedioCompra { get; init; }
    public decimal ValorTotalAlocado { get; init; }
    public DateTime DataPrimeiraCompra { get; init; }
}