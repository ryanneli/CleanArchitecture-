using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/clientes")]
public sealed class ClienteController(ClienteService clienteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        return Ok(await clienteService.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] CriarClienteDto dto)
    {
        var cliente = await clienteService.AddAsync(dto);
        if (cliente is null)
            return Conflict(new { message = "Já existe um cliente com este nome." });

        return CreatedAtAction(nameof(GetAll), cliente);
    }
}
