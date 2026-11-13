namespace Application.DTO;

public sealed record ClienteDto(int Id, string Nome, string? FotoUrl);

public sealed record CriarClienteDto(string Nome, string? FotoUrl);
