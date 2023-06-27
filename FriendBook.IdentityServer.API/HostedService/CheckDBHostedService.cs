using FriendBook.IdentityServer.API.DAL;

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

            if (await _appDBContext.Database.EnsureCreatedAsync())
            {
                _appDBContext.UpdateDatabase();
            }

            return;
        }
    }
}