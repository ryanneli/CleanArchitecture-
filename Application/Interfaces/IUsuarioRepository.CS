using Dominio.DTOs;
using Dominio.Entities;

namespace Application.Interfaces
{
  public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuarios?>> GetAllAsync();

        Task<Usuarios?> GetByIdAsync(int id);

        Task<Usuarios?> GetByEmailAsync(string email);

        Task AddAsync(Usuarios usuario);
        void Update(Usuarios usuario);
        void Delete(Usuarios usuario);

        Task<bool> SaveChangesAsync();

    }
}