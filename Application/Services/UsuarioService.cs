using Application.DTO;
using Application.Interfaces;
using Dominio.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class UsuarioService
    {

        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<Usuarios> _userManager;
        private readonly string _photosPath;
        private readonly string _publicBaseUrl;

        public UsuarioService(
            IUsuarioRepository usuario,
            UserManager<Usuarios> userManager,
            IConfiguration configuration)
        {
            _usuarioRepository = usuario;
            _userManager = userManager;
            _configuration = configuration;
            _photosPath = Path.GetFullPath(
                configuration["FileStorage:UserPhotosPath"] ?? Path.Combine("wwwroot", "uploads", "usuarios"),
                Directory.GetCurrentDirectory());
            _publicBaseUrl = (configuration["PublicBaseUrl"] ?? "http://localhost:5021").TrimEnd('/');
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios
                .Where(u => u is not null)
                .Select(u => ToDto(u!));

        }


        public async Task<bool> AddAsync(CriarUsuarioDto dto)
        {
            var email = string.IsNullOrWhiteSpace(dto.Email)
                ? $"legacy-{Guid.NewGuid():N}@invalid.local"
                : dto.Email;
            var usuario = new Usuarios
            {
                Nome = dto.Nome,
                Email = email,
                UserName = email,
                Cargo = dto.Cargo,
                Perfil = dto.Perfil,
                FotoUrl = SalvarFoto(dto.FotoUrl),
                PermitirEditarObra = dto.PermitirEditarObra,
                PermitirExcluirObra = dto.PermitirExcluirObra,
                PermitirExcluirUsuario = dto.PermitirExcluirUsuario,
                PermitirAdicionarUsuario = dto.PermitirAdicionarUsuario,
                PermitirAdicionarCliente = dto.PermitirAdicionarCliente,
                PermitirMoverAlocarFuncionario = dto.PermitirMoverAlocarFuncionario,
                PermitirEditarFuncionario = dto.PermitirEditarFuncionario
            };

            var result = await _userManager.CreateAsync(usuario, dto.senha);
            if (!result.Succeeded)
                ExcluirFotoLocal(usuario.FotoUrl);
            return result.Succeeded;

        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var item = await _usuarioRepository.GetByIdAsync(id);
            if (item is null) return null;

            return ToDto(item);
        }

        public async Task<bool> UpdateAsync(int id, AtualizarUsuarioDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario is null) return false;

            usuario.Nome = dto.Nome;
            var emailExterno = $"legacy-{id}@invalid.local";
            usuario.Email = string.IsNullOrWhiteSpace(dto.Email) ? emailExterno : dto.Email;
            usuario.UserName = usuario.Email;
            usuario.Cargo = dto.Cargo;
            usuario.Perfil = dto.Perfil;
            var fotoAnterior = usuario.FotoUrl;
            usuario.FotoUrl = SalvarFoto(dto.FotoUrl) ?? fotoAnterior;
            usuario.PermitirEditarObra = dto.PermitirEditarObra;
            usuario.PermitirExcluirObra = dto.PermitirExcluirObra;
            usuario.PermitirExcluirUsuario = dto.PermitirExcluirUsuario;
            usuario.PermitirAdicionarUsuario = dto.PermitirAdicionarUsuario;
            usuario.PermitirAdicionarCliente = dto.PermitirAdicionarCliente;
            usuario.PermitirMoverAlocarFuncionario = dto.PermitirMoverAlocarFuncionario;
            usuario.PermitirEditarFuncionario = dto.PermitirEditarFuncionario;

            if (string.IsNullOrEmpty(usuario.SecurityStamp))
            {   
            usuario.SecurityStamp = Guid.NewGuid().ToString();
            }

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded) return false;

            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                result = await _userManager.ResetPasswordAsync(usuario, token, dto.Senha);
            }

            if (result.Succeeded && usuario.FotoUrl != fotoAnterior)
                ExcluirFotoLocal(fotoAnterior);

            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario is null) return false;

            var result = await _userManager.DeleteAsync(usuario);
            return result.Succeeded;
        }


        public async Task<string?> LoginAsync(LoginDto user)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(user.Email);
            if (usuario is null) return null;

            var passwordValid = await _userManager.CheckPasswordAsync(usuario, user.Senha);
            if (!passwordValid) return null;

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret não foi configurado.");
            var issuer = jwtSettings["Issuer"]
                ?? throw new InvalidOperationException("JwtSettings:Issuer não foi configurado.");
            var audience = jwtSettings["Audience"]
                ?? throw new InvalidOperationException("JwtSettings:Audience não foi configurado.");
            var key = Encoding.UTF8.GetBytes(secretKey);


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email!),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("perfil", usuario.Perfil ?? string.Empty),
                new Claim("permitirEditarObra", usuario.PermitirEditarObra.ToString()),
                new Claim("permitirExcluirObra", usuario.PermitirExcluirObra.ToString()),
                new Claim("permitirAdicionarCliente", usuario.PermitirAdicionarCliente.ToString()),
                new Claim("permitirMoverAlocarFuncionario", usuario.PermitirMoverAlocarFuncionario.ToString()),
                new Claim("permitirEditarFuncionario", usuario.PermitirEditarFuncionario.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8), // Token expira em 8 horas
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email);
            if (usuario is null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var result = await _userManager.ResetPasswordAsync(usuario, token, dto.Senha);
            return result.Succeeded;
        }

        public bool ValidarChaveReset(string? chave)
        {
            var chaveConfigurada = _configuration["Security:AdminPasswordResetKey"];
            if (string.IsNullOrWhiteSpace(chaveConfigurada) || string.IsNullOrEmpty(chave))
                return false;

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(chave),
                Encoding.UTF8.GetBytes(chaveConfigurada));
        }

        private UsuarioDto ToDto(Usuarios usuario)
        {
            return new UsuarioDto(
                usuario.Id,
                usuario.Nome,
                usuario.Cargo,
                NormalizarFotoUrl(usuario.FotoUrl),
                EmailExibicao(usuario.Email),
                usuario.Perfil,
                usuario.PermitirEditarObra,
                usuario.PermitirExcluirObra,
                usuario.PermitirExcluirUsuario,
                usuario.PermitirAdicionarUsuario,
                usuario.PermitirAdicionarCliente,
                usuario.PermitirMoverAlocarFuncionario,
                usuario.PermitirEditarFuncionario);
        }

        private string? SalvarFoto(string? foto)
        {
            if (string.IsNullOrWhiteSpace(foto))
                return null;

            if (!foto.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return foto;

            var separator = foto.IndexOf(";base64,", StringComparison.OrdinalIgnoreCase);
            if (separator <= 5)
                throw new ArgumentException("A foto deve ser uma imagem Base64 válida.");

            var mime = foto[5..separator].ToLowerInvariant();
            var extensao = mime switch
            {
                "image/png" => ".png",
                "image/jpeg" or "image/jpg" => ".jpg",
                "image/webp" => ".webp",
                _ => throw new ArgumentException("A foto deve ser PNG, JPEG ou WebP.")
            };

            byte[] conteudo;
            try
            {
                conteudo = Convert.FromBase64String(foto[(separator + 8)..]);
            }
            catch (FormatException)
            {
                throw new ArgumentException("A foto enviada não possui um conteúdo Base64 válido.");
            }

            if (conteudo.Length > 5 * 1024 * 1024)
                throw new ArgumentException("A foto deve ter no máximo 5 MB.");

            Directory.CreateDirectory(_photosPath);
            var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
            File.WriteAllBytes(Path.Combine(_photosPath, nomeArquivo), conteudo);
            return $"{_publicBaseUrl}/uploads/usuarios/{nomeArquivo}";
        }

        private string? NormalizarFotoUrl(string? fotoUrl)
        {
            if (string.IsNullOrWhiteSpace(fotoUrl))
                return null;

            if (Uri.TryCreate(fotoUrl, UriKind.Absolute, out var uri) &&
                uri.AbsolutePath.StartsWith("/uploads/usuarios/", StringComparison.OrdinalIgnoreCase))
            {
                return $"{_publicBaseUrl}{uri.AbsolutePath}";
            }

            return fotoUrl;
        }

        private static string? EmailExibicao(string? email)
        {
            return email?.EndsWith("@invalid.local", StringComparison.OrdinalIgnoreCase) == true
                ? null
                : email;
        }

        private void ExcluirFotoLocal(string? fotoUrl)
        {
            if (!Uri.TryCreate(fotoUrl, UriKind.Absolute, out var uri) ||
                !uri.AbsolutePath.StartsWith("/uploads/usuarios/", StringComparison.OrdinalIgnoreCase))
                return;

            var nomeArquivo = Path.GetFileName(uri.AbsolutePath);
            var caminho = Path.Combine(_photosPath, nomeArquivo);
            if (File.Exists(caminho))
                File.Delete(caminho);
        }
        

    }

}