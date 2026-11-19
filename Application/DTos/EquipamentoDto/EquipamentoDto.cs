using Dominio.Entities.Enums;

namespace Application.DTO;

public sealed record CriarEquipamentoDto(
    string Nome,
    string? CodigoPatrimonio,
    string? FotoUrl,
    OrigemEquipamento Origem,
    StatusEquipamento Status,
    string? FornecedorLocacao,
    DateTime? DataInicioLocacao,
    DateTime? DataFimLocacao);

public sealed record AtualizarEquipamentoDto(
    string Nome,
    string? CodigoPatrimonio,
    string? FotoUrl,
    OrigemEquipamento Origem,
    StatusEquipamento Status,
    string? FornecedorLocacao,
    DateTime? DataInicioLocacao,
    DateTime? DataFimLocacao);

public sealed record AlterarStatusEquipamentoDto(StatusEquipamento Status);

public sealed record AlocarEquipamentoDto(int ObraId);

public sealed record EquipamentoDto(
    int Id,
    string Nome,
    string? CodigoPatrimonio,
    string? FotoUrl,
    OrigemEquipamento Origem,
    StatusEquipamento Status,
    string? FornecedorLocacao,
    DateTime? DataInicioLocacao,
    DateTime? DataFimLocacao,
    int? ObraId,
    string? NumeroOs,
    string? NomeCliente,
    bool ComDefeito,
    bool Proprio,
    DateTime CriadoEm);
