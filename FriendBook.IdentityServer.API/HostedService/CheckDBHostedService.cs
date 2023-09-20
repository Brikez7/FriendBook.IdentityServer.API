using FriendBook.IdentityServer.API.DAL;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.IdentityServer.API.HostedService
{
    public class CheckDBHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IdentityContext? _appDBContext;

        public CheckDBHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public  async Task StartAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _appDBContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();

            if (!await _appDBContext.Database.CanConnectAsync() || (await _appDBContext.Database.GetPendingMigrationsAsync(stoppingToken)).Any())
            {
                await _appDBContext.Database.MigrateAsync(stoppingToken);
                return;
            }

            return;
        }
    }
}