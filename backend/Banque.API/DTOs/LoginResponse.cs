namespace Banque.API.DTOs
{
    public record LoginResponse(string Token, DateTime ExpiresAt);
}
