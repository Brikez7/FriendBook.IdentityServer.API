using FriendBook.IdentityServer.API.Domain.Settings;
using FriendBook.IdentityServer.Tests.WebAppFactories.DbBuilders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace FriendBook.IdentityServer.Tests.WebAppFactories
{
    internal class WebHostFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>
    where TProgram : class where TDbContext : DbContext
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private readonly RedisContainer _redisContainer;
        public WebHostFactory()
        {
            _postgresContainer = ContainerBuilderPostgres.CreatePostgreSQLContainer();
            _redisContainer = ContainerBuilderRedis.CreateRedisContainer();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.Test.json");

            builder.ConfigureAppConfiguration((conf) =>  conf.AddJsonFile(configPath));

            builder.ConfigureTestServices(services =>
            {
                services.ReplaceDbContext<TDbContext>(_postgresContainer.GetConnectionString());
                services.ReplaceConnectionRedis(_redisContainer.GetConnectionString());
            });

            builder.UseEnvironment("Test");
        }
        internal async Task InitializeAsync() 
        {
            var task1  = _postgresContainer.StartAsync();
            var task2 = _redisContainer.StartAsync();

            await Task.WhenAll(task1, task2);  
        }
        internal async Task ClearData() 
        {
            var dbPostges = Services.GetRequiredService<TDbContext>();
            var dbRedis = Services.GetRequiredService<IDistributedCache>();

            var task2 = dbPostges.Database.EnsureDeletedAsync();
            var task1 = dbRedis.RemoveAsync("*");

            await Task.WhenAll(task1,task2);

            await dbPostges.Database.MigrateAsync();
        }
        public override async ValueTask DisposeAsync() 
        {
            await _postgresContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();

            await base.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
    internal static class ServiceCollectionExtensions
    {
        public static void ReplaceDbContext<T>(this IServiceCollection services, string newConnectionPostgres) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null) services.Remove(descriptor);

            Console.WriteLine(newConnectionPostgres);
            services.AddDbContext<T>(options => { options.UseNpgsql(newConnectionPostgres); });
        }
        internal static void ReplaceConnectionRedis(this IServiceCollection services, string newConnectionRedis) 
        {
            var connection = $"{newConnectionRedis},Password={ContainerBuilderRedis.Password}";

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connection;
            });

            var existingOptions = services.BuildServiceProvider().GetRequiredService<IOptions<RedisSettings>>();
            RedisSettings value = existingOptions.Value;
            value.Host = connection;

            var updatedOptions = Options.Create(value);
            services.AddSingleton(updatedOptions);
        }
    }
}
