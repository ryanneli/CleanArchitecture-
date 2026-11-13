namespace Application.DTO
{
    public sealed record UsuarioDto(
        int Id,
        string Nome,
        string? Cargo,
        string? FotoUrl,
        string? Email,
        string Perfil,
        bool PermitirEditarObra = false,
        bool PermitirExcluirObra = false,
        bool PermitirExcluirUsuario = false,
        bool PermitirAdicionarUsuario = false,
        bool PermitirAdicionarCliente = false,
        bool PermitirMoverAlocarFuncionario = false,
        bool PermitirEditarFuncionario = false);

    public sealed record CriarUsuarioDto(
        string Nome,
        string Email,
        string senha,
        string? Cargo = null,
        string Perfil = "Padrao",
        string? FotoUrl = null,
        bool PermitirEditarObra = false,
        bool PermitirExcluirObra = false,
        bool PermitirExcluirUsuario = false,
        bool PermitirAdicionarUsuario = false,
        bool PermitirAdicionarCliente = false,
        bool PermitirMoverAlocarFuncionario = false,
        bool PermitirEditarFuncionario = false);
    public sealed record AtualizarUsuarioDto(
        string Nome,
        string? Email,
        string? Senha,
        string? Cargo,
        string Perfil,
        string? FotoUrl,
        bool PermitirEditarObra,
        bool PermitirExcluirObra,
        bool PermitirExcluirUsuario,
        bool PermitirAdicionarUsuario,
        bool PermitirAdicionarCliente,
        bool PermitirMoverAlocarFuncionario,
        bool PermitirEditarFuncionario);
}