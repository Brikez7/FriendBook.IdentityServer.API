using FriendBook.IdentityServer.API.DAL;
using Microsoft.EntityFrameworkCore;

namespace FriendBook.IdentityServer.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSingleton(builder.Configuration);
            builder.AddRepositores();
            builder.AddServices();
            builder.AddJWT();

            builder.AddHostedServices();

            builder.Services.AddDbContext<IdentityServerContext>(opt => opt.UseNpgsql(
            builder.Configuration.GetConnectionString(IdentityServerContext.NameConnection)));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(builder => builder
               .WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod());

            app.UseCors();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}