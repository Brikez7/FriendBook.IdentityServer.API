using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.CustomClaims;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.Settings;
using FriendBook.IdentityServer.API.Domain.UserToken;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class RegistrationService : IRegistrationService
    {
        protected readonly ILogger<RegistrationService> _logger;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IRedisLockService _redisLockService;
        private readonly JWTSettings _jwtSettings;
        public RegistrationService(ILogger<RegistrationService> logger, IAccountService accountService, ITokenService tokenService, IPasswordService passwordService, IOptions<JWTSettings> jwtOptions,
            IRedisLockService redisLockService)
        {
            _logger = logger;
            _accountService = accountService;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _jwtSettings = jwtOptions.Value;
            _redisLockService = redisLockService;
        }
        public async Task<BaseResponse<ResponseAuthenticated>> Registration(RequestAccount accountDTO)
        {
            var responseAccount = await _accountService.GetAccount(x => x.Login == accountDTO.Login);
            if (responseAccount.StatusCode == StatusCode.EntityNotFound)
            {
                _passwordService.CreatePasswordHash(accountDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var newAccount = new Account(accountDTO, Convert.ToBase64String(passwordSalt), Convert.ToBase64String(passwordHash));

                var responseNewAccount = await _accountService.CreateAccount(newAccount);

                newAccount = responseNewAccount.Data;
                var authToken = (await Authenticate(accountDTO)).Data;
                return new StandartResponse<ResponseAuthenticated>()
                {
                    Data = authToken,
                    StatusCode = StatusCode.AccountCreate
                };
            }
            return new StandartResponse<ResponseAuthenticated>()
            {
                Message = "Account with login already exists",
                StatusCode = StatusCode.AccountAlraedyExists
            };
        }

        public async Task<BaseResponse<ResponseAuthenticated>> Authenticate(RequestAccount accountDTO)
        {
            var account = (await _accountService.GetAccount(x => x.Login == accountDTO.Login)).Data;
            if (account == null ||
                !_passwordService.VerifyPasswordHash(accountDTO.Password, Convert.FromBase64String(account.Password), Convert.FromBase64String(account.Salt)))
            {
                return new StandartResponse<ResponseAuthenticated>()
                {
                    Message = "Account not found",
                    StatusCode = StatusCode.ErrorAuthenticate
                };
            }

            return new StandartResponse<ResponseAuthenticated>()
            {
                Data = _tokenService.GenerateAuthenticatedToken(new TokenAuth(account.Login,(Guid)account.Id!)).Data,
                StatusCode = StatusCode.AccountAuthenticate
            };
        }

        public async Task<BaseResponse<string?>> GetAccessToken(TokenAuth tokenAuth, string refreshToken)
        {
            var claimsRT = _tokenService.GetPrincipalFromExpiredToken(refreshToken, _jwtSettings.RefreshTokenSecretKey);

            var userSecretNumber = claimsRT?.Data?.Claims.FirstOrDefault(x => x.Type == CustomClaimType.SecretNumber)?.Value;
            if (claimsRT?.StatusCode == StatusCode.TokenRead || userSecretNumber != null)
            {
                var secretNumberResponse = await _redisLockService.GetSecretNumber(tokenAuth.Id.ToString());
                if (secretNumberResponse.StatusCode == StatusCode.RedisReceive && secretNumberResponse.Data == userSecretNumber) 
                {
                    var newAccessToken = _tokenService.GenerateAccessToken(tokenAuth);
                    return new StandartResponse<string?> {Data = refreshToken, StatusCode = StatusCode.AccountAuthenticateByRT };
                }
                return new StandartResponse<string?> { Message = "Secret number not found please go to authorization", StatusCode = StatusCode.ErrorAuthenticate };
            }
            return  new StandartResponse<string?> { Message = "Refresh token or access token is not valid", StatusCode = StatusCode.ErrorAuthenticate };
        }
    }
}
