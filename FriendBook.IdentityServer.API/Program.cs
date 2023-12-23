using FriendBook.IdentityServer.API.BLL.GrpcServices;
using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.IdentityServer.API
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(builder.Configuration);

            builder.AddRepositories();
            builder.AddValidators();
            builder.AddServices();
            builder.AddGrpcServices();
            
            builder.AddAuthProperty();

            builder.AddRedisPropperty();

            builder.Services.AddDbContext<IdentityContext>(opt => opt.UseNpgsql(
                builder.Configuration.GetConnectionString(IdentityContext.NameConnection)));

            builder.AddHostedServices();

            builder.Services.Configure<AppUISetting>(builder.Configuration.GetSection("AppUISetting"));

            builder.Services.AddGrpcReflection();
            builder.Services.AddGrpc()
                .AddJsonTranscoding()
                ;

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddSwagger();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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