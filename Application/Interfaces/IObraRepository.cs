
using Dominio.Entities;

namespace Application.Interfaces
{
    public interface IObraRepository
    {
        Task<IEnumerable<Obra?>> GetAllAsync();
        Task<Obra?> GetByIdAsync(int id);
        Task AddAsync(Obra obra);
        void Update(Obra obra);
        void Delete(Obra obra);

        Task<bool> SaveChangesAsync();

    }
}