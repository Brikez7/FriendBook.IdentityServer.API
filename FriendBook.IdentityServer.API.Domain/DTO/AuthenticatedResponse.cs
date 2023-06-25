namespace FriendBook.IdentityServer.API.Domain.DTO
{
    public class AuthenticatedTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public AuthenticatedTokenResponse(string? accessToken, string? refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
