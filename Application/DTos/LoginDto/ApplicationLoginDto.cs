namespace Application.DTO
{
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }

    public sealed record ResetPasswordDto(string Email, string Senha);
}