using FriendBook.IdentityServer.API.DAL;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.IdentityServer.API.HostedService
{
    public class CheckDBHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IdentityContext? _appDBContext;

        public CheckDBHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _appDBContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();

            if (await _appDBContext.Database.EnsureCreatedAsync(stoppingToken))
            {
                await _appDBContext.Database.MigrateAsync(stoppingToken);
                return;
            }

            if ((await _appDBContext.Database.GetPendingMigrationsAsync(stoppingToken)).Any())
            {
                await _appDBContext.Database.MigrateAsync(stoppingToken);
            }

            return;
        }
    }
}