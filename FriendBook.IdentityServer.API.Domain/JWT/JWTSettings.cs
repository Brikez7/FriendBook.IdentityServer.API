namespace FriendBook.IdentityServer.API.Domain.JWT
{
    public class JWTSettings
    {
        public string AccessTokenSecretKey { get; set; } = null!;
        public string RefreshTokenSecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public double AccessTokenExpirationMinutes { get; set; } 
        public double RefreshTokenExpirationMinutes { get; set; } 
    }
}
