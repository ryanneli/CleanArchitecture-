using System.ComponentModel.DataAnnotations;

namespace Dominio.DTOs
{
    public sealed record CriarObraDto(
        [Required(ErrorMessage = "O número da OS é obrigatório.")]
        [StringLength(255)]
        string OS,

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(255)]
        string Cliente,

        [StringLength(255)]
        string? Localizacao,

        [Required(ErrorMessage = "O status da obra é obrigatório.")]
        string Status,

        string? TipoCadastro,
        string? ImagemUrl,
        string? FotoClienteUrl,
        bool VisitaTecnica,
        string? Observacao,
        string? Grupo,
        DateTimeOffset? DataInicio,
        DateTimeOffset? DataPrevisaoTermino
    );

    // DTO usado para listar as obras em formato de cards (incluindo a equipe alocada)
    public sealed record ObraCardDto(
        int Id,
        string OS,
        string Cliente,
        string Localizacao,
        string Status,
        string? ImagemUrl,
        string? FotoClienteUrl,
        bool VisitaTecnica,
        string? Observacao,
        DateTimeOffset? DataInicio,
        DateTimeOffset? DataPrevisaoTermino,
        List<UsuarioDtoExibir> Equipe
    );

    // DTO focado em exibir os detalhes completos de uma obra específica
    public sealed record ObraDetalhesDto(
        int Id,
        string OS,
        string Cliente,
        string Localizacao,
        string Status,
        string TipoCadastro,
        string? ImagemUrl,
        string? FotoClienteUrl,
        bool VisitaTecnica,
        string? Observacao,
        string? Grupo,
        DateTime DataCriacao,
        DateTimeOffset? DataInicio,
        DateTimeOffset? DataPrevisaoTermino
    );

    // DTO usado na ação de arrastar e soltar (Drag & Drop) para alocar um funcionário na obra
    public sealed record AlocarFuncionarioDto(
        [Required(ErrorMessage = "O ID do funcionário é obrigatório.")]
        int FuncionarioId
    );
}