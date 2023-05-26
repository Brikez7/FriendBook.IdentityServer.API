using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.JWT;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class RegistrationService : IRegistrationService
    {
        protected readonly ILogger<RegistrationService> _logger;
        private readonly JWTSettings _options;
        private readonly IAccountService _accountService;

        public RegistrationService(ILogger<RegistrationService> logger, IOptions<JWTSettings> options, IAccountService accountService)
        {
            _logger = logger;
            _options = options.Value;
            _accountService = accountService;
        }
        public async Task<BaseResponse<string>> Registration(AccountDTO DTO)
        {
            try
            {
                ///
                Account account = new Account(DTO);
                ///
                var accountOnRegistration = (await _accountService.GetAccount(x => x.Login == DTO.Login)).Data;
                if (accountOnRegistration != null)
                {
                    return new StandartResponse<string>()
                    {
                        Message = "Account with that login alredy exist",
                        StatusCode = StatusCode.AccountWithLoginExists
                    };
                }
                CreatePasswordHash(DTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
                /// Add mapper
                var newAccount = new Account(DTO, Convert.ToBase64String(passwordSalt), Convert.ToBase64String(passwordHash));
                ///
                newAccount = (await _accountService.CreateAccount(newAccount)).Data;
                return new StandartResponse<string>()
                {
                    Data = (await Authenticate(DTO)).Data,
                    StatusCode = StatusCode.AccountCreate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Registration] : {ex.Message}");
                return new StandartResponse<string>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<string>> Authenticate(AccountDTO DTO)
        {
            try
            {
                var account = await _accountService.GetAccount(x => x.Login == DTO.Login);
                if (account.Data == null ||
                    !VerifyPasswordHash(DTO.Password, Convert.FromBase64String(account.Data.Password), Convert.FromBase64String(account.Data.Salt)))
                {
                    return new StandartResponse<string>()
                    {
                        Message = "account not found",
                        StatusCode = StatusCode.ErrorAuthenticate
                    };
                }
                string token = GetToken(account.Data);
                return new StandartResponse<string>()
                {
                    Data = token,
                    StatusCode = StatusCode.AccountAuthenticate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Authenticate] : {ex.Message}");
                return new StandartResponse<string>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public string GetToken(Account account)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,account.Login),
                new Claim(CustomClaimType.Password, account.Password),
                new Claim(CustomClaimType.AccountId, account.Id.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(JWTSettings.StartJWTTokenLifeTime)),
                    notBefore: DateTime.Now,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public IEnumerable<Claim> ReadToken(string tokenstring)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
            var claims = token.Claims;
            return claims;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string Password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
