using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Net;

namespace FriendBook.IdentityServer.API.HostedService.Grpc
{
    public class GrpcEndpoinListenHostService : BackgroundService
    {
        private readonly GrpcSettings _grpcSettings;

        public GrpcEndpoinListenHostService(IOptions<GrpcSettings> grpcSettings)
        {
            _grpcSettings = grpcSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureKestrel(options =>
                {
                    options.Listen(IPAddress.Any, _grpcSettings.IdentityGrpcHost, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                })
                .UseKestrel()
                .UseStartup<GrpcHostedServerStartup>();
            })
            .Build()
            .StartAsync(stoppingToken);
        }
    }
}
