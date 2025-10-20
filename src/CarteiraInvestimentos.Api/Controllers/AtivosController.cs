using CarteiraInvestimentos.Application.Commands.ComprarAtivo;
using CarteiraInvestimentos.Application.Commands.VenderAtivo;
using CarteiraInvestimentos.Application.DTOs;
using CarteiraInvestimentos.Application.Queries.ObterExtrato;
using CarteiraInvestimentos.Application.Queries.ObterResumoCarteira;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarteiraInvestimentos.Api.Controllers;

[ApiController]
[Route("api/ativos")]
[Produces("application/json")]
public class AtivosController : ControllerBase
{
    private readonly IMediator _mediator;

    public AtivosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra uma operação de compra de ativo
    /// </summary>
    /// <param name="command">Dados da compra (código, quantidade, preço unitário)</param>
    /// <returns>Confirmação da operação</returns>
    /// <response code="200">Compra registrada com sucesso</response>
    /// <response code="400">Dados inválidos (quantidade/preço ≤ 0, código vazio)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("comprar")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ComprarAtivo([FromBody] ComprarAtivoCommand command)
    {
        await _mediator.Send(command);
        return Ok("Operação de compra registrada com sucesso.");
    }

    /// <summary>
    /// Registra uma operação de venda de ativo
    /// </summary>
    /// <param name="command">Dados da venda (código, quantidade)</param>
    /// <returns>Confirmação da operação</returns>
    /// <response code="200">Venda registrada com sucesso</response>
    /// <response code="400">Dados inválidos ou saldo insuficiente</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("vender")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VenderAtivo([FromBody] VenderAtivoCommand command)
    {
        await _mediator.Send(command);
        return Ok("Operação de venda registrada com sucesso.");
    }

    /// <summary>
    /// Obtém o resumo da carteira de investimentos 
    /// </summary>
    /// <returns>O valor total da carteira e a lista de saldos de ativos</returns>
    /// <response code="200">Resumo retornado com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("resumo")]
    [ProducesResponseType(typeof(ResumoCarteiraDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterResumoCarteira()
    {
        var query = new ObterResumoCarteiraQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém o extrato de todas as transações 
    /// </summary>
    /// <returns>Uma lista de todas as transações (compras e vendas) ordenadas por data</returns>
    /// <response code="200">Extrato retornado com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("extrato")]
    [ProducesResponseType(typeof(List<TransacaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterExtrato()
    {
        var query = new ObterExtratoQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
}

/// <summary>
/// Resposta de erro padrão
/// </summary>
public class ErrorResponse
{
    /// <summary>Código HTTP do erro</summary>
    public int StatusCode { get; set; }

    /// <summary>Mensagem descritiva do erro</summary>
    public required string Message { get; set; }
}