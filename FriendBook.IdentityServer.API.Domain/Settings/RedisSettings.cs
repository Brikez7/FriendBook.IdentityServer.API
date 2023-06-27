namespace FriendBook.IdentityServer.API.Domain.Settings
{
    public class RedisSettings
    {
        public const string Name = "RedisSettings";
        public string Host { get; set; } = null!;
        public string Resource { get; set; } = null!;
        public TimeSpan Expiry { get; set; }
        public TimeSpan Wait { get; set; }
        public TimeSpan Retry { get; set; }
        public TimeSpan StoreDuration { get; set; }
    }
}
