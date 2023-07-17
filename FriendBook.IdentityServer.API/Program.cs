using FriendBook.IdentityServer.API.BLL.GrpcServices;
using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FriendBook.IdentityServer.API
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(builder.Configuration);

            builder.AddRepositories();
            builder.AddServices();
            builder.AddValidators();

            builder.AddAuthProperty();

            builder.AddRedisPropperty();

            builder.Services.AddDbContext<IdentityContext>(opt => opt.UseNpgsql(
                builder.Configuration.GetConnectionString(IdentityContext.NameConnection)));

            builder.AddHostedServices();

            builder.Services.Configure<AppUISetting>(builder.Configuration.GetSection("AppUISetting"));

            builder.Services.AddGrpcReflection();
            builder.Services.AddGrpc().AddJsonTranscoding();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddGrpcSwagger();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.1",new OpenApiInfo { Version = "v1.1" });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1.1/swagger.json", "FriendBook.IdentityServer.API v1.1");
                });
            }

            app.MapGrpcService<GrpcPublicAccountService>();
            app.MapGrpcService<GrpcPublicContactService>();

            app.MapGrpcReflectionService();

            app.AddCorsUI();

            app.UseCors();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}