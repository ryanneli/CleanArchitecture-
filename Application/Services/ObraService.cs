using Application.Interfaces;
using Dominio.DTOs;
using Dominio.Entities;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class ObraService
    {
        private readonly IObraRepository _obraRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IUsuarioRepository _usuarioRepository; // Repositório para buscar funcionários/usuários
        private readonly string _publicBaseUrl;

        public ObraService(
            IObraRepository obraRepository, 
            IClienteRepository clienteRepository,
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration)
        {
            _obraRepository = obraRepository;
            _clienteRepository = clienteRepository;
            _usuarioRepository = usuarioRepository;
            _publicBaseUrl = (configuration["PublicBaseUrl"] ?? "http://localhost:5021").TrimEnd('/');
        }

        // 1. Listar todas as obras em formato de Card
        public async Task<IEnumerable<ObraCardDto>> ObterTodasCardsAsync()
        {
            var obras = await _obraRepository.ObterTodasComEquipeAsync();

            return obras.Select(o => new ObraCardDto(
                o.id,
                o.OS,
                o.Cliente,
                o.Localizacao ?? string.Empty,
                o.StatusObra.ToString(),
                o.ImagemUrl,
                o.FotoClienteUrl,
                o.VisitaTecnica,
                o.Observacao,
                o.DataInicio,
                o.DataPrevisaoTermino,
                o.Equipe.Select(e => new UsuarioDtoExibir(
                    id: e.Id,
                    Nome: e.Nome,
                    Cargo: e.Cargo,
                    FotoUrl: NormalizarFotoUrl(e.FotoUrl),
                    Email: e.Email,
                    Perfil: e.Perfil,
                    PermitirEditarObra: e.PermitirEditarObra,
                    PermitirExcluirObra: e.PermitirExcluirObra,
                    PermitirExcluirUsuario: e.PermitirExcluirUsuario,
                    PermitirAdicionarUsuario: e.PermitirAdicionarUsuario,
                    PermitirAdicionarCliente: false,
                    PermitirMoverAlocarFuncionario: e.PermitirMoverAlocarFuncionario,
                    PermitirEditarFuncionario: e.PermitirEditarFuncionario
                )).ToList()
            ));
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

        // 2. Obter detalhes de uma obra específica por ID
        public async Task<ObraDetalhesDto?> ObterDetalhesPorIdAsync(int id)
        {
            var obra = await _obraRepository.ObterPorIdAsync(id);
            if (obra == null) return null;

            return new ObraDetalhesDto(
                obra.id,
                obra.OS,
                obra.Cliente,
                obra.Localizacao ?? string.Empty,
                obra.StatusObra.ToString(),
                obra.TipoCadastro.ToString(),
                obra.ImagemUrl,
                obra.FotoClienteUrl,
                obra.VisitaTecnica,
                obra.Observacao,
                obra.Grupo,
                obra.DataCriacao,
                obra.DataInicio,
                obra.DataPrevisaoTermino
            );
        }

        // 3. Criar uma nova obra
        public async Task<ObraDetalhesDto> CriarObraAsync(CriarObraDto dto)
        {
            Enum.TryParse<StatusObra>(dto.Status, true, out var statusEnum);
            Enum.TryParse<TipoCadastroObra>(dto.TipoCadastro, true, out var tipoCadastroEnum);

            var cliente = await _clienteRepository.ObterPorNomeInicioAsync(dto.Cliente);

            var novaObra = new Obra
            {
                OS = dto.OS,
                Cliente = dto.Cliente,
                Localizacao = dto.Localizacao,
                StatusObra = statusEnum,
                TipoCadastro = tipoCadastroEnum,
                ImagemUrl = dto.ImagemUrl,
                FotoClienteUrl = cliente?.FotoUrl ?? dto.FotoClienteUrl,
                VisitaTecnica = dto.VisitaTecnica,
                Observacao = dto.Observacao,
                Grupo = dto.Grupo,
                DataInicio = dto.DataInicio,
                DataPrevisaoTermino = dto.DataPrevisaoTermino,
                DataCriacao = DateTime.Now
            };

            await _obraRepository.AdicionarAsync(novaObra);

            return new ObraDetalhesDto(
                novaObra.id,
                novaObra.OS,
                novaObra.Cliente,
                novaObra.Localizacao ?? string.Empty,
                novaObra.StatusObra.ToString(),
                novaObra.TipoCadastro.ToString(),
                novaObra.ImagemUrl,
                novaObra.FotoClienteUrl,
                novaObra.VisitaTecnica,
                novaObra.Observacao,
                novaObra.Grupo,
                novaObra.DataCriacao,
                novaObra.DataInicio,
                novaObra.DataPrevisaoTermino
            );
        }

        // 4. Atualizar uma obra existente
        public async Task<bool> AtualizarObraAsync(int id, CriarObraDto dto)
        {
            var obra = await _obraRepository.ObterPorIdAsync(id);
            if (obra == null) return false;

            Enum.TryParse<StatusObra>(dto.Status, true, out var statusEnum);
            Enum.TryParse<TipoCadastroObra>(dto.TipoCadastro, true, out var tipoCadastroEnum);

            var cliente = await _clienteRepository.ObterPorNomeInicioAsync(dto.Cliente);

            obra.OS = dto.OS;
            obra.Cliente = dto.Cliente;
            obra.Localizacao = dto.Localizacao;
            obra.StatusObra = statusEnum;
            obra.TipoCadastro = tipoCadastroEnum;
            obra.ImagemUrl = dto.ImagemUrl;
            obra.FotoClienteUrl = cliente?.FotoUrl ?? dto.FotoClienteUrl;
            obra.VisitaTecnica = dto.VisitaTecnica;
            obra.Observacao = dto.Observacao;
            obra.Grupo = dto.Grupo;
            obra.DataInicio = dto.DataInicio;
            obra.DataPrevisaoTermino = dto.DataPrevisaoTermino;

            await _obraRepository.AtualizarAsync(obra);
            return true;
        }

        // 5. Alocar funcionário na equipe da obra (Drag & Drop)
        public async Task<bool> AlocarFuncionarioAsync(int obraId, int funcionarioId)
        {
            var obra = await _obraRepository.ObterComEquipePorIdAsync(obraId);
            var usuario = await _usuarioRepository.GetByIdAsync(funcionarioId);

            if (obra == null || usuario == null) return false;

            obra.Equipe ??= new List<Usuarios>();

            // Verifica se o funcionário já não faz parte da equipe para evitar duplicidade
            if (!obra.Equipe.Any(u => u.Id == funcionarioId))
            {
                obra.Equipe.Add(usuario);
                await _obraRepository.AtualizarAsync(obra);
            }

            return true;
        }

        // 6. Remover funcionário da equipe da obra
        public async Task<bool> RemoverFuncionarioAsync(int obraId, int funcionarioId)
        {
            var obra = await _obraRepository.ObterComEquipePorIdAsync(obraId);
            if (obra == null) return false;

            var usuario = obra.Equipe.FirstOrDefault(u => u.Id == funcionarioId);
            if (usuario != null)
            {
                obra.Equipe.Remove(usuario);
                await _obraRepository.AtualizarAsync(obra);
                return true;
            }

            return false;
        }

        // 7. Deletar obra
        public async Task<bool> DeletarObraAsync(int id)
        {
            var obra = await _obraRepository.ObterPorIdAsync(id);
            if (obra == null) return false;

            await _obraRepository.DeletarAsync(obra);
            return true;
        }
    }
}