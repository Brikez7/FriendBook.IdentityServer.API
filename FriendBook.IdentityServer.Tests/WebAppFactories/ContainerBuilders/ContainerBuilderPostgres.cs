using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace FriendBook.IdentityServer.Tests.WebAppFactories.ContainerBuilders
{
    internal class ContainerBuilderPostgres
    {
        public const string User = "TestPostgres";
        public const string Password = "TestPostgres54321!";
        public const string Database = "Test_FriendBook_IdentityServer";
        public const string ExposedPort = "5433";
        public const string PortBinding = "5432";
        private const string ImagePostgres = "postgres:latest";

        public static PostgreSqlContainer CreatePostgreSQLContainer()
        {
            var dbBuilderPostgre = new PostgreSqlBuilder();

            return dbBuilderPostgre
                .WithName($"PostgresDB.Identity.{Guid.NewGuid():N}")
                .WithImage(ImagePostgres)
                .WithHostname($"PostgresHost.Identity.{Guid.NewGuid():N}")
                .WithExposedPort(ExposedPort)
                .WithPortBinding(PortBinding, true)
                .WithUsername(User)
                .WithPassword(Password)
                .WithDatabase(Database)
                .WithTmpfsMount("/pgdata")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted($"export PGPASSWORD='{Password}';psql -U {User} -d '{Database}' -c \"select 1\""))
                .WithCleanUp(true)
                .Build();
        }
    }
}
