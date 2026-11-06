namespace Dominio.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? FotoUrl { get; set; } = string.Empty; 
    }
}