using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Dominio.Entities;
using Application.Interfaces;

namespace Infrastructure.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepositorio(AppDbContext contexto)
        {
            _context = contexto;
        }

        public async Task<IEnumerable<Usuarios>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuarios?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuarios?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(Usuarios usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public void Update(Usuarios usuario)
        {
            _context.Usuarios.Update(usuario);
        }

        public void Delete(Usuarios usuario)
        {
            _context.Usuarios.Remove(usuario);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}