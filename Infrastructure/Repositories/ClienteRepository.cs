using Dominio.Entities;
using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            return await _context.Clientes.AsNoTracking().ToListAsync();
        }

        public async Task<Cliente?> ObterPorNomeInicioAsync(string nomeInicio)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => nomeInicio.StartsWith(c.Nome));
        }

        public async Task<bool> ExistePorNomeAsync(string nome)
        {
            var nomeNormalizado = nome.Trim().ToLower();
            return await _context.Clientes
                .AnyAsync(c => c.Nome.Trim().ToLower() == nomeNormalizado);
        }

        public async Task AdicionarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task<Cliente?> ObterPorIdAsync(int id)
        {
            return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExcluirAsync(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
    }
}