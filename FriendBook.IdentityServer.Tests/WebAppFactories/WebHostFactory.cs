using FriendBook.IdentityServer.API.HostedService.Grpc;
using FriendBook.IdentityServer.Tests.WebAppFactories.DbBuilders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace FriendBook.IdentityServer.Tests.WebAppFactories
{
    public class WebHostFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>
    where TProgram : class where TDbContext : DbContext
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private readonly RedisContainer _redisContainer;
        public WebHostFactory()
        {
            _postgresContainer = ContainerBuilderPostgre.CreatePostgreSQLContainer();
            _redisContainer = ContainerBuilderRedis.CreateRedisContainer();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.Test.json");

            builder.ConfigureAppConfiguration((conf) =>  conf.AddJsonFile(configPath));

            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<TDbContext>();
                services.AddDbContext<TDbContext>(options => { options.UseNpgsql(_postgresContainer.GetConnectionString()); });

                var grpcs = services.SingleOrDefault(d =>
                    d.ImplementationType == typeof(GrpcHostedServerStartup));

                var grpc = services.SingleOrDefault(d =>
                    d.ImplementationType == typeof(GrpcEndpointListenHostService));

                services.Remove(grpc);
                services.Remove(grpcs);
            });

            builder.UseEnvironment("Test");
        }
        public async Task InitializeAsync() 
        {
            var task1  = _postgresContainer.StartAsync();
            var task2 = _redisContainer.StopAsync();

            await Task.WhenAll(task1,task2);
        }
        public override async ValueTask DisposeAsync() 
        {
            await _postgresContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
    public static class ServiceCollectionExtensions
    {
        public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null) services.Remove(descriptor);
        }
    }
}
