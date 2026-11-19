using Application.DTO;
using Application.Interfaces;
using Dominio.Entities;
using Dominio.Entities.Enums;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public sealed class EquipamentoService(
    IEquipamentoRepository equipamentoRepository,
    IObraRepository obraRepository,
    IConfiguration configuration) : IEquipamentoService
{
    private readonly string _photosPath = Path.GetFullPath(
        configuration["FileStorage:EquipmentPhotosPath"] ?? Path.Combine("wwwroot", "uploads", "equipamentos"),
        Directory.GetCurrentDirectory());
    private readonly string _publicBaseUrl = (configuration["PublicBaseUrl"] ?? "http://localhost:5021").TrimEnd('/');

    public async Task<IEnumerable<EquipamentoDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var equipamentos = await equipamentoRepository.ObterTodosAsync(cancellationToken);
        return equipamentos.Select(ToDto);
    }

    public async Task<EquipamentoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var equipamento = await equipamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return equipamento is null ? null : ToDto(equipamento);
    }

    public async Task<EquipamentoDto> CriarAsync(CriarEquipamentoDto dto, CancellationToken cancellationToken = default)
    {
        var equipamento = new Equipamento(
            dto.Nome,
            dto.Origem,
            dto.CodigoPatrimonio,
            SalvarFoto(dto.FotoUrl),
            dto.FornecedorLocacao,
            dto.DataInicioLocacao,
            dto.DataFimLocacao);

        if (dto.Status != StatusEquipamento.Disponivel)
        {
            if (dto.Status == StatusEquipamento.EmUso)
                throw new ArgumentException("Um equipamento novo não pode ser criado como EmUso.");
            equipamento.AlterarStatus(dto.Status);
        }

        await equipamentoRepository.AdicionarAsync(equipamento, cancellationToken);
        return ToDto(equipamento);
    }

    public async Task<bool> AtualizarAsync(int id, AtualizarEquipamentoDto dto, CancellationToken cancellationToken = default)
    {
        var equipamento = await equipamentoRepository.ObterPorIdAsync(id, cancellationToken);
        if (equipamento is null) return false;

        equipamento.AtualizarDados(
            dto.Nome,
            dto.Origem,
            dto.CodigoPatrimonio,
            SalvarFoto(dto.FotoUrl) ?? equipamento.FotoUrl,
            dto.FornecedorLocacao,
            dto.DataInicioLocacao,
            dto.DataFimLocacao);

        if (dto.Status != equipamento.Status)
        {
            if (dto.Status == StatusEquipamento.EmUso && !equipamento.ObraId.HasValue)
                throw new ArgumentException("Não é possível marcar o equipamento como EmUso sem uma obra vinculada.");

            equipamento.AlterarStatus(dto.Status);
        }

        await equipamentoRepository.AtualizarAsync(equipamento, cancellationToken);
        return true;
    }

    public async Task<bool> AlterarStatusAsync(int id, AlterarStatusEquipamentoDto dto, CancellationToken cancellationToken = default)
    {
        var equipamento = await equipamentoRepository.ObterPorIdAsync(id, cancellationToken);
        if (equipamento is null) return false;

        if (dto.Status == StatusEquipamento.EmUso && !equipamento.ObraId.HasValue)
            throw new ArgumentException("Não é possível marcar o equipamento como EmUso sem uma obra vinculada.");

        equipamento.AlterarStatus(dto.Status);
        await equipamentoRepository.AtualizarAsync(equipamento, cancellationToken);
        return true;
    }

    public async Task<bool> AlocarAsync(int id, AlocarEquipamentoDto dto, CancellationToken cancellationToken = default)
    {
        var equipamento = await equipamentoRepository.ObterPorIdAsync(id, cancellationToken);
        if (equipamento is null) return false;

        var obra = await obraRepository.ObterPorIdAsync(dto.ObraId);
        if (obra is null) return false;

        equipamento.AlocarNaObra(dto.ObraId);
        await equipamentoRepository.AtualizarAsync(equipamento, cancellationToken);
        return true;
    }

    public async Task<bool> DesalocarAsync(int id, CancellationToken cancellationToken = default)
    {
        var equipamento = await equipamentoRepository.ObterPorIdAsync(id, cancellationToken);
        if (equipamento is null) return false;

        equipamento.DesalocarDaObra(equipamento.Origem == OrigemEquipamento.Locado);
        await equipamentoRepository.AtualizarAsync(equipamento, cancellationToken);
        return true;
    }

    public async Task<bool> ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var equipamento = await equipamentoRepository.ObterPorIdAsync(id, cancellationToken);
        if (equipamento is null) return false;

        await equipamentoRepository.RemoverAsync(equipamento, cancellationToken);
        return true;
    }

    private string? SalvarFoto(string? foto)
    {
        if (string.IsNullOrWhiteSpace(foto))
            return null;

        if (!foto.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return foto;

        var separator = foto.IndexOf(";base64,", StringComparison.OrdinalIgnoreCase);
        if (separator <= 5)
            throw new ArgumentException("A foto deve ser uma imagem Base64 válida.");

        var mime = foto[5..separator].ToLowerInvariant();
        var extensao = mime switch
        {
            "image/png" => ".png",
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/webp" => ".webp",
            _ => throw new ArgumentException("A foto deve ser PNG, JPEG ou WebP.")
        };

        byte[] conteudo;
        try
        {
            conteudo = Convert.FromBase64String(foto[(separator + 8)..]);
        }
        catch (FormatException)
        {
            throw new ArgumentException("A foto enviada não possui um conteúdo Base64 válido.");
        }

        if (conteudo.Length > 5 * 1024 * 1024)
            throw new ArgumentException("A foto deve ter no máximo 5 MB.");

        Directory.CreateDirectory(_photosPath);
        var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
        File.WriteAllBytes(Path.Combine(_photosPath, nomeArquivo), conteudo);
        return $"{_publicBaseUrl}/uploads/equipamentos/{nomeArquivo}";
    }

    private static EquipamentoDto ToDto(Equipamento equipamento)
    {
        return new EquipamentoDto(
            equipamento.Id,
            equipamento.Nome,
            equipamento.CodigoPatrimonio,
            equipamento.FotoUrl,
            equipamento.Origem,
            equipamento.Status,
            equipamento.FornecedorLocacao,
            equipamento.DataInicioLocacao,
            equipamento.DataFimLocacao,
            equipamento.ObraId,
            equipamento.Obra?.OS,
            equipamento.Obra?.Cliente,
            equipamento.Status == StatusEquipamento.ComDefeito,
            equipamento.Origem == OrigemEquipamento.Proprio,
            equipamento.CriadoEm);
    }
}
