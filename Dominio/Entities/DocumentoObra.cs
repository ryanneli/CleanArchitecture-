using System.Dynamic;

namespace Dominio.Entities
{
    public class DocumentosObra
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
        
        public string Nome { get; set; }

        public bool Concluido { get; set; }

        public int ObraId { get; set; }

        public Obra Obra { get; set; }
        
    }
}