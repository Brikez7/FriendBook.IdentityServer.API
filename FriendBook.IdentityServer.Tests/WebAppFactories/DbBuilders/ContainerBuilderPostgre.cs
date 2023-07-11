using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace FriendBook.IdentityServer.Tests.WebAppFactories.DbBuilders
{
     class ContainerBuilderPostgre
    {
        public const string User = "postgres";
        public const string Password = "pg";
        public const string NameDatabase = "Test_FreindBook_IdentityServer";
        public const string Port = "5432";
        public static PostgreSqlContainer CreatePostgreSQLContainer()
        {
            var dbBuidlerPostgre = new PostgreSqlBuilder();

            return dbBuidlerPostgre
                .WithName(Guid.NewGuid().ToString("N"))
                .WithImage("postgres:latest")
                .WithHostname(Guid.NewGuid().ToString("N"))
                .WithExposedPort(Port)
                .WithPortBinding(Port, true)
                .WithUsername(User)
                .WithPassword(Password)
                .WithDatabase(NameDatabase)
                .WithTmpfsMount("/pgdata")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("psql -U postgres -c \"select 1\""))
                .WithCleanUp(true)
                .Build();
        }
    }
}
