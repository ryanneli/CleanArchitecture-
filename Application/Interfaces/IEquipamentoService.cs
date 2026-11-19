using Application.DTO;

namespace Application.Interfaces;

public interface IEquipamentoService
{
    Task<IEnumerable<EquipamentoDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<EquipamentoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EquipamentoDto> CriarAsync(CriarEquipamentoDto dto, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(int id, AtualizarEquipamentoDto dto, CancellationToken cancellationToken = default);
    Task<bool> AlterarStatusAsync(int id, AlterarStatusEquipamentoDto dto, CancellationToken cancellationToken = default);
    Task<bool> AlocarAsync(int id, AlocarEquipamentoDto dto, CancellationToken cancellationToken = default);
    Task<bool> DesalocarAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default);
}
