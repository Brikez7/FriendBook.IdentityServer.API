using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Net;

namespace FriendBook.IdentityServer.API.HostedService.Grpc
{
    public class GrpcEndpointListenHostService : BackgroundService
    {
        private readonly GrpcSettings _grpcSettings;
        private readonly IConfiguration _configuration;
        public GrpcEndpointListenHostService(IOptions<GrpcSettings> grpcSettings, IConfiguration configuration)
        {
            _grpcSettings = grpcSettings.Value;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureAppConfiguration((config) => config.AddConfiguration(_configuration));

            await builder.ConfigureWebHostDefaults(builder =>
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
