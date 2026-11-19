using Dominio.Entities;
using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EquipamentoRepository : IEquipamentoRepository
    {
        private readonly AppDbContext _context; // use o nome real do seu DbContext

    public EquipamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Equipamento?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Equipamentos
            .Include(e => e.Obra)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<Equipamento>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipamentos
            .AsNoTracking()
            .Include(e => e.Obra)
            .OrderByDescending(e => e.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Equipamento>> ObterPorObraIdAsync(int obraId, CancellationToken cancellationToken = default)
    {
        return await _context.Equipamentos
            .AsNoTracking()
            .Where(e => e.ObraId == obraId)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Equipamento equipamento, CancellationToken cancellationToken = default)
    {
        await _context.Equipamentos.AddAsync(equipamento, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Equipamento equipamento, CancellationToken cancellationToken = default)
    {
        _context.Equipamentos.Update(equipamento);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Equipamento equipamento, CancellationToken cancellationToken = default)
    {
        _context.Equipamentos.Remove(equipamento);
        await _context.SaveChangesAsync(cancellationToken);
    }
    }
}