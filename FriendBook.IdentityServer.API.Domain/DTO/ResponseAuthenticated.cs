namespace FriendBook.IdentityServer.API.Domain.DTO
{
    public class ResponseAuthenticated
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public ResponseAuthenticated(string? accessToken, string? refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
