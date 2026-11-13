
using Dominio.Entities;

namespace Application.Interfaces
{
    public interface IObraRepository
    {
        Task<IEnumerable<Obra>> ObterTodasComEquipeAsync();
        Task<Obra?> ObterPorIdAsync(int id);
        Task<Obra?> ObterComEquipePorIdAsync(int id);
        Task AdicionarAsync(Obra obra);
        Task AtualizarAsync(Obra obra);
        Task DeletarAsync(Obra obra);

        Task<bool> SaveChangesAsync();

    }
}