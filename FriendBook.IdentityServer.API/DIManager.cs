using FluentValidation;
using FriendBook.IdentityServer.API.BackgroundHostedService;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.BLL.Services;
using FriendBook.IdentityServer.API.DAL.Repositories.Implemetations;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Validators.AccountVlidators;
using FriendBook.IdentityServer.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FriendBook.IdentityServer.API
{
    public static class DIManager
    {
        public static void AddRepositores(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IAccountRepository, AccountRepository>();
            webApplicationBuilder.Services.AddScoped<IContactRepository, ContactRepository>();
        }
        public static void AddValidators(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IValidator<AccountDTO>, ValidatorAccountDTO>();
            webApplicationBuilder.Services.AddScoped<IValidator<UserContactDTO>, ValidatorUserContactDTO>();

            webApplicationBuilder.Services.AddScoped<IValidationService<AccountDTO>, ValidationService<AccountDTO>>();
            webApplicationBuilder.Services.AddScoped<IValidationService<UserContactDTO>, ValidationService<UserContactDTO>>();
        }
        public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IAccountService, AccountService>();
            webApplicationBuilder.Services.AddScoped<IRegistrationService, RegistrationService>();
            webApplicationBuilder.Services.AddScoped<IContactService, ContactService>();
            webApplicationBuilder.Services.AddScoped<ITokenService, TokenService>();
            webApplicationBuilder.Services.AddScoped<IPasswordService, PasswordService>();
            webApplicationBuilder.Services.AddScoped<IUserAccessTokenService, UserAccessTokenService>();
        }
        public static void AddRedisPropperty(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = webApplicationBuilder.Configuration.GetSection("RedisOptions:Host").Value;
            });
        }
        public static void AddJWT(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.Configure<JWTSettings>(webApplicationBuilder.Configuration.GetSection("JWTSettings"));

            var jwtSettings = webApplicationBuilder.Configuration.GetSection("JWTSettings").Get<JWTSettings>() 
                ?? throw new ArgumentNullException("JWTSettings is null");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecretKey!));

            webApplicationBuilder.Services.AddAuthentication(options =>
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
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = JwtHelper.CustomLifeTimeValidator
                };
            });
        }
        public static void AddHostedServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddHostedService<CheckDBHostedService>();
        }
        public static void AddMiddleware(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}