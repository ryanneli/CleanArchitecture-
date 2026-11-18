namespace Application.DTO
{
    public sealed record AtualizarDocumentoDto(int Id, string Sigla, string Nome, bool Concluido);
}