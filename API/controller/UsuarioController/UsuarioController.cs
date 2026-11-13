using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers // Ajustado para refletir a pasta correta
{
    [ApiController]
    [Authorize]
    [Route("api/usuarios")]

    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        // Injeção de dependência
        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddAsync([FromBody] CriarUsuarioDto usuario)
        {
            // O serviço retorna bool (true para sucesso, false para falha)
            var sucesso = await _usuarioService.AddAsync(usuario);

            if (!sucesso)
                return BadRequest(new { message = "Não foi possível cadastrar o usuário." });

            return Ok(new { message = "Usuário criado com sucesso!" });
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] AtualizarUsuarioDto usuario)
        {
            var sucesso = await _usuarioService.UpdateAsync(id, usuario);
            return sucesso ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var sucesso = await _usuarioService.DeleteAsync(id);
            return sucesso ? NoContent() : NotFound();
        }
    }
}