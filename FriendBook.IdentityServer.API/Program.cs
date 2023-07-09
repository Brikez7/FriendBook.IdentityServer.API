using FriendBook.IdentityServer.API.DAL;
using FriendBook.IdentityServer.API.Domain.Settings;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.IdentityServer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton(builder.Configuration);

            builder.AddRepositores();
            builder.AddAuthProperty();
            builder.AddServices();
            builder.AddValidators();
            builder.AddRedisPropperty();

            builder.Services.AddDbContext<IdentityContext>(opt => opt.UseNpgsql(
                builder.Configuration.GetConnectionString(IdentityContext.NameConnection)));
            builder.AddHostedServices();

            builder.Services.Configure<AppUISetting>(builder.Configuration.GetSection("AppUISetting"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.AddCorsUI();

            app.UseCors();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}