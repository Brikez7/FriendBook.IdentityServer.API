namespace FriendBook.IdentityServer.API.Domain.DTO
{
    public class ResponseAuthenticate
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public ResponseAuthenticate(string? accessToken, string? refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
