using Dominio.Entities;

namespace Application.Interfaces
{
    public interface IEquipamentoRepository
    {
        Task<Equipamento?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Equipamento>> ObterTodosAsync(CancellationToken cancellationToken = default);

        Task<List<Equipamento>> ObterPorObraIdAsync(int ObraId, CancellationToken cancellationToken = default);

        Task AdicionarAsync(Equipamento equipamento, CancellationToken cancellationToken = default);

        Task AtualizarAsync(Equipamento equipamento, CancellationToken cancellationToken = default);
        Task RemoverAsync(Equipamento equipamento, CancellationToken cancellationToken = default);

    }    
}