using Testcontainers.Redis;

namespace FriendBook.IdentityServer.Tests.WebAppFactories.DbBuilders
{
    public class ContainerBuilderRedis
    {
        public const string Port = "5432";
        public const string Password = "123456";
        public static RedisContainer CreateRedisContainer()
        {
            var redisBuilder = new RedisBuilder();

            return redisBuilder.WithHostname($"localhost:{Port},password={Password}")
                               .WithImage("redis:latest")
                               .WithPortBinding($"{Port}:{Port}")
                               .Build();
        }
    }
}
