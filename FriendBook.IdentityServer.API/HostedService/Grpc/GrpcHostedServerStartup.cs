using FriendBook.IdentityServer.API.BLL.GrpcServices;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.BLL.Services;
using FriendBook.IdentityServer.API.BLL.Services.Implementations;
using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.DAL.Repositories.Implemetations;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FriendBook.IdentityServer.API.HostedService.Grpc
{
    public class GrpcHostedServerStartup
    {
        private IConfiguration _configuration;

        public GrpcHostedServerStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            AddJWT(_configuration, services);
            services.AddAuthorization();

            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddScoped<IUserAccountService, UserAccountService>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IContactService, ContactService>();

            services.AddDbContext<IdentityContext>(opt => opt.UseNpgsql(
                _configuration.GetConnectionString(IdentityContext.NameConnection)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GrpcPublicAccountService>();
                endpoints.MapGrpcService<GrpcPublicContactService>();
            });
        }
        public static void AddJWT(IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            var jwtSettings = configuration.GetSection("JWTSettings").Get<JWTSettings>()
                ?? throw new ArgumentNullException("JWTSettings is null");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecretKey!));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true
                };
            });
        }
    }
}
