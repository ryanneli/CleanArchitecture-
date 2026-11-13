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
    }
}