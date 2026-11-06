namespace Dominio.Entities
{
    public class Usuarios
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;
        public string? senha { get; set; } = string.Empty;

        public string Perfil { get; set; } = string.Empty;

        public string? Cargo { get; set; } = string.Empty;

        public string? FotoUrl { get; set; } = string.Empty;

        //Permissões deacesso
        public bool PermitirEditarObra { get; set; } = false;
        public bool PermitirExcluirObra { get; set; } = false;
        public bool PermitirExcluirUsuario { get; set; } = false;
        public bool PermitirEditarFuncionario { get; set; } = false;
        public bool PermitirAdicionarUsuario { get; set; } = false;
        public bool PermitirAdicionarCliente { get; set; } = false;
        public bool PermitirMoverAlocarFuncionario { get; set; } = false;

        
        public string? AvatarUrl => FotoUrl ?? $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(Nome)}&background=0d6efd&color=fff&size=128";

        public ICollection<Obra> Obras { get; set; } = new List<Obra>();

    }
}