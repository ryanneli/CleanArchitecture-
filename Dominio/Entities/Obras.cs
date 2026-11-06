namespace Dominio.Entities
{

        public enum TipoCadastroObra
        {
            Simples = 1,
            Completo = 2
        }

        public enum StatusObra
        {
            Planejamento = 1,
            EmAndamento = 2,
            Concluida = 3,
            Pausado = 4,
            Cancelado = 5
        }

    public class Obra
    {
        public int id { get; set; }
        public string OS { get; set; } = string.Empty;

        public string Cliente { get; set; } = string.Empty;

        public string Localizacao { get; set; } = string.Empty;

        public string? Grupo { get; set; }

        public string? Observacao { get; set; }

        public StatusObra StatusObra { get; set; } = StatusObra.EmAndamento;
        public TipoCadastroObra TipoCadastro { get; set; } = TipoCadastroObra.Simples;
        public string? ImagemUrl { get; set; }

        public string? FotoClienteUrl { get; set; }

        public bool VisitaTecnica { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTimeOffset? DataInicio { get; set; }

        public DateTimeOffset? DataPrevisaoTermino { get; set; }

        public ICollection<Usuarios> Equipe { get; set; } = new List<Usuarios>();

    }
 }
