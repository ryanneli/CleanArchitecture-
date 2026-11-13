using Application.DTO;
using Application.Interfaces;
using Dominio.Entities;

namespace Application.Services;

public sealed class ClienteService(IClienteRepository clienteRepository)
{
    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var clientes = await clienteRepository.ObterTodosAsync();
        return clientes.Select(cliente => new ClienteDto(cliente.Id, cliente.Nome, cliente.FotoUrl));
    }

    public async Task<ClienteDto?> AddAsync(CriarClienteDto dto)
    {
        var nome = dto.Nome.Trim();
        if (await clienteRepository.ExistePorNomeAsync(nome))
            return null;

        var cliente = new Cliente { Nome = nome, FotoUrl = dto.FotoUrl };
        await clienteRepository.AdicionarAsync(cliente);
        return new ClienteDto(cliente.Id, cliente.Nome, cliente.FotoUrl);
    }
}
