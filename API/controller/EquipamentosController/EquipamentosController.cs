using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/equipamentos")]
public sealed class EquipamentosController(IEquipamentoService equipamentoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipamentoDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await equipamentoService.ListarAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EquipamentoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var equipamento = await equipamentoService.ObterPorIdAsync(id, cancellationToken);
        return equipamento is null ? NotFound() : Ok(equipamento);
    }

    [HttpPost]
    public async Task<ActionResult<EquipamentoDto>> Create([FromBody] CriarEquipamentoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var equipamento = await equipamentoService.CriarAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = equipamento.Id }, equipamento);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AtualizarEquipamentoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var atualizado = await equipamentoService.AtualizarAsync(id, dto, cancellationToken);
            return atualizado ? NoContent() : NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> AlterarStatus(int id, [FromBody] AlterarStatusEquipamentoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var alterado = await equipamentoService.AlterarStatusAsync(id, dto, cancellationToken);
            return alterado ? NoContent() : NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:int}/alocar")]
    public async Task<IActionResult> Alocar(int id, [FromBody] AlocarEquipamentoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var alocado = await equipamentoService.AlocarAsync(id, dto, cancellationToken);
            return alocado ? NoContent() : NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:int}/desalocar")]
    public async Task<IActionResult> Desalocar(int id, CancellationToken cancellationToken)
    {
        var desalocado = await equipamentoService.DesalocarAsync(id, cancellationToken);
        return desalocado ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var excluido = await equipamentoService.ExcluirAsync(id, cancellationToken);
        return excluido ? NoContent() : NotFound();
    }
}
