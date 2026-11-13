using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public AuthController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _usuarioService.LoginAsync(dto);

            if (token == null)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
            }

            return Ok(new { token = token });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto dto,
            [FromHeader(Name = "X-Password-Reset-Key")] string? resetKey)
        {
            if (!_usuarioService.ValidarChaveReset(resetKey))
                return Unauthorized(new { message = "Chave de recuperação inválida." });

            var sucesso = await _usuarioService.ResetPasswordAsync(dto);
            return sucesso
                ? NoContent()
                : NotFound(new { message = "Usuário não encontrado ou senha rejeitada." });
        }
    }
}