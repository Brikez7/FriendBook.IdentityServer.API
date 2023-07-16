using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Net;

namespace FriendBook.IdentityServer.API.HostedService.Grpc
{
    public class GrpcEndpointListenHostService : IHostedService
    {
        private readonly GrpcSettings _grpcSettings;
        private readonly IConfiguration _configuration;
        private IHost? _host;

        public GrpcEndpointListenHostService(IOptions<GrpcSettings> grpcSettings, IConfiguration configuration)
        {
            _grpcSettings = grpcSettings.Value;
            _configuration = configuration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureAppConfiguration((config) => config.AddConfiguration(_configuration));

            _host = builder.ConfigureWebHostDefaults(builder =>
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
            .Build();

            await _host.StartAsync(cancellationToken);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_host != null)
            {
                using var cts = new CancellationTokenSource();
                await _host.StopAsync(cts.Token);
            }
        }
    }
}


