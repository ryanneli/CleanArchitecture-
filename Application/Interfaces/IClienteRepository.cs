
using Dominio.Entities;
namespace Application.Interfaces
{
  public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllAsync();

        Task<Cliente?> GetByIdAsync(int id);

        void AddAsync(Cliente cliente);

        void Update(Cliente cliente);

        void Delete(Cliente cliente);

        Task<bool> SaveChangesAsync();

    }
}