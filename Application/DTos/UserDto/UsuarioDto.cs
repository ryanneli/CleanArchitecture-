using System.ComponentModel.DataAnnotations;

namespace Dominio.DTOs
{
    //criar novo usuario
    public class RegistroUsuarioDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "o perfil é obrigatório")]
        [StringLength(100)]
        public required string Perfil { get; set; }

        [StringLength(100)]
        public string? Cargo { get; set; }

        public string? FotoUrl { get; set; }

        public bool PermitirEditarObra { get; set; }
        public bool PermitirExcluirObra { get; set; }

        public bool PermitirExcluirUsuario { get; set; }

        public bool PermitirAdicionarUsuario { get; set; }

        public bool PermitirMoverAlocarFuncionario { get; set; }

        public bool PermitirEditarFuncionario { get; set; }

    }

    // DTO usado para atualizar os dados de um usuário existente
    public sealed record AtualizarUsuarioDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public required string Nome { get; init; }

        [StringLength(255)]
        public string? Email { get; init; }

        [StringLength(255)]
        public string? Senha { get; init; }

        [Required(ErrorMessage = "O perfil é obrigatório.")]
        [StringLength(100)]
        public required string Perfil { get; init; }

        [StringLength(100)]
        public string? Cargo { get; init; }

        public string? FotoUrl { get; init; }

        public bool PermitirEditarObra { get; init; }
        public bool PermitirExcluirObra { get; init; }
        public bool PermitirExcluirUsuario { get; init; }
        public bool PermitirAdicionarUsuario { get; init; }
        public bool PermitirAdicionarCliente { get; init; }
        public bool PermitirMoverAlocarFuncionario { get; init; }
        public bool PermitirEditarFuncionario { get; init; }
    }

    public sealed record UsuarioDtoExibir(int id, string Nome, string? Cargo, string? FotoUrl, string? Email, string Perfil, bool PermitirEditarObra = false,
        bool PermitirExcluirObra = false,
        bool PermitirExcluirUsuario = false,
        bool PermitirAdicionarUsuario = false,
        bool PermitirAdicionarCliente = false,
        bool PermitirMoverAlocarFuncionario = false,
        bool PermitirEditarFuncionario = false);
    

    }
