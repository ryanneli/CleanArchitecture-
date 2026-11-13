using Application.DTO;
using Application.Services;
using Dominio.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/obras")]
public sealed class ObraController(ObraService obraService) : ControllerBase
{
    [HttpGet("cards")]
    [ProducesResponseType(typeof(IEnumerable<ObraCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ObraCardDto>>> GetCards()
    {
        return Ok(await obraService.ObterTodasCardsAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ObraDetalhesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ObraDetalhesDto>> GetById(int id)
    {
        var obra = await obraService.ObterDetalhesPorIdAsync(id);
        return obra is null ? NotFound() : Ok(obra);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ObraDetalhesDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ObraDetalhesDto>> Create([FromBody] CriarObraDto dto)
    {
        var obra = await obraService.CriarObraAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = obra.Id }, obra);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] CriarObraDto dto)
    {
        var updated = await obraService.AtualizarObraAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await obraService.DeletarObraAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{obraId:int}/equipe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddEmployee(
        int obraId,
        [FromBody] AlocarFuncionarioDto dto)
    {
        var allocated = await obraService.AlocarFuncionarioAsync(obraId, dto.FuncionarioId);
        return allocated ? NoContent() : NotFound();
    }

    [HttpDelete("{obraId:int}/equipe/{funcionarioId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveEmployee(int obraId, int funcionarioId)
    {
        var removed = await obraService.RemoverFuncionarioAsync(obraId, funcionarioId);
        return removed ? NoContent() : NotFound();
    }
}