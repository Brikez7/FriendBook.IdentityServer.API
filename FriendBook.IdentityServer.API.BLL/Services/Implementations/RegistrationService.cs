using FriendBook.IdentityServer.API.BLL.Helpers;
using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL.Repositories.Implemetations;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
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

namespace FriendBook.IdentityServer.API.BLL.Services.Implementations
{
    public class RegistrationService : IRegistrationService
    {
        protected readonly ILogger<RegistrationService> _logger;
        private readonly IUserAccountService _userAccountService;
        private readonly ITokenService _tokenService;
        private readonly IRedisLockService _redisLockService;
        private readonly IAccountRepository _accountRepository;
        private readonly JWTSettings _jwtSettings;
        public RegistrationService(ILogger<RegistrationService> logger, IAccountRepository accountRepository, IUserAccountService accountService, ITokenService tokenService, IOptions<JWTSettings> jwtOptions,
            IRedisLockService redisLockService)
        {
            _logger = logger;
            _userAccountService = accountService;
            _tokenService = tokenService;
            _jwtSettings = jwtOptions.Value;
            _redisLockService = redisLockService;
            _accountRepository = accountRepository;
        }
        public async Task<BaseResponse<ResponseAuthenticated>> Registration(RequestAccount accountDTO)
        {
            var responseAccount = await _userAccountService.GetAccount(x => x.Login == accountDTO.Login);
            if (responseAccount.StatusCode == StatusCode.EntityNotFound)
            {
                PasswordHelper.CreatePasswordHash(accountDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var newAccount = new Account(accountDTO, Convert.ToBase64String(passwordSalt), Convert.ToBase64String(passwordHash));

                newAccount = await _accountRepository.AddAsync(newAccount);
                await _accountRepository.SaveAsync();

                var authToken = (await Authenticate(accountDTO)).Data;
                return new StandartResponse<ResponseAuthenticated>()
                {
                    Data = authToken,
                    StatusCode = StatusCode.UserRegistered
                };
            }
            return new StandartResponse<ResponseAuthenticated>()
            {
                Message = "Account with login already exists",
                StatusCode = StatusCode.UserAlreadyExists
            };
        }

        public async Task<BaseResponse<ResponseAuthenticated>> Authenticate(RequestAccount accountDTO)
        {
            var account = (await _userAccountService.GetAccount(x => x.Login == accountDTO.Login)).Data;
            if (account == null ||
                !PasswordHelper.VerifyPasswordHash(accountDTO.Password, Convert.FromBase64String(account.Password), Convert.FromBase64String(account.Salt)))
            {
                return new StandartResponse<ResponseAuthenticated>()
                {
                    Message = "Account not found",
                    StatusCode = StatusCode.ErrorAuthenticate
                };
            }

            var dataAT = new DataAccessToken(account.Login, (Guid)account.Id!);
            var responseTokens = _tokenService.GenerateAuthenticatedToken(dataAT, out string secretNumber);
            _ = _redisLockService.SetSecretNumber(secretNumber, "Token:" + account.Id.ToString());

            return new StandartResponse<ResponseAuthenticated>()
            {
                Data = responseTokens,
                StatusCode = StatusCode.UserAuthenticated
            };
        }

        public async Task<BaseResponse<string>> GetAccessToken(DataAccessToken tokenAuth, string refreshToken)
        {
            var claimsRT = _tokenService.GetPrincipalFromExpiredToken(refreshToken, _jwtSettings.RefreshTokenSecretKey);

            var userSecretNumber = claimsRT?.Data?.Claims.FirstOrDefault(x => x.Type == CustomClaimType.SecretNumber)?.Value;
            if (claimsRT?.StatusCode == StatusCode.TokenRead || userSecretNumber != null)
            {
                var secretNumberResponse = await _redisLockService.GetSecretNumber("Token:" + tokenAuth.Id.ToString());
                if (secretNumberResponse.StatusCode == StatusCode.RedisReceive && secretNumberResponse.Data == userSecretNumber)
                {
                    var newAccessToken = _tokenService.GenerateAccessToken(tokenAuth);
                    return new StandartResponse<string> { Data = newAccessToken, StatusCode = StatusCode.UserAuthenticatedByRT };
                }
                return new StandartResponse<string> { Message = "Secret number not found please go to authorization", StatusCode = StatusCode.ErrorAuthenticate };
            }
            return new StandartResponse<string> { Message = "Refresh token or access token is not valid", StatusCode = StatusCode.ErrorAuthenticate };
        }
    }
}
