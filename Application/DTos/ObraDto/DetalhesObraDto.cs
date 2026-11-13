using System.Runtime.InteropServices;

namespace Application.DTO
{
    public record ObraDetalhes(int Id, string OS, string Localizacao, string Status, string TipoCadastro, string? ImagemUrl, string? FotoClienteUrl, bool VisitaTecnica, string? Observacao, string? Grupo, DateTime DataCriacao, DateTime? DataInicio, DateTime? DataPrevisãoTermino);
}