
using Dominio.Entities;
namespace Application.Interfaces
{
  public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ObterTodosAsync();
        Task<Cliente?> ObterPorNomeInicioAsync(string nomeInicio);
        Task<bool> ExistePorNomeAsync(string nome);
        Task AdicionarAsync(Cliente cliente);
        Task<Cliente?> ObterPorIdAsync(int id);
        Task<bool> ExcluirAsync(Cliente cliente);
        

        

    }
}