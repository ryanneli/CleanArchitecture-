using Dominio.Entities;
using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ObraRepository : IObraRepository
    {
        private readonly AppDbContext _context;

        public ObraRepository(AppDbContext contexto)
        {
            _context = contexto;
        }

        public async Task<IEnumerable<Obra>> ObterTodasComEquipeAsync()
        {
            return await _context.Obras.AsNoTracking().Include(o => o.Equipe).ToListAsync();

        }

        public async Task<Obra?> ObterPorIdAsync(int id)
        {
            return await _context.Obras.FindAsync(id);
        }

        public async Task<Obra?> ObterComEquipePorIdAsync(int id)
        {
            return await _context.Obras.Include(o => o.Equipe).FirstOrDefaultAsync(o => o.id == id);
        }

        public async Task AdicionarAsync(Obra obra)
        {
            await _context.Obras.AddAsync(obra);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Obra obra)
        {
            _context.Obras.Update(obra);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(Obra obra)
        {
            _context.Obras.Remove(obra);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}