﻿using FriendBook.IdentityServer.API.BLL.Interfaces;
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
        private readonly JWTSettings _jwtSettings;
        public RegistrationService(ILogger<RegistrationService> logger, IAccountService accountService, ITokenService tokenService, IPasswordService passwordService, IOptions<JWTSettings> jwtOptions)
        {
            _logger = logger;
            _accountService = accountService;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _jwtSettings = jwtOptions.Value;
        }
        public async Task<BaseResponse<ResponseAuthenticated>> Registration(AccountDTO accountDTO)
        {
            var responseAccount = await _accountService.GetAccount(x => x.Login == accountDTO.Login);
            if (responseAccount.Data is null)
            {
                _passwordService.CreatePasswordHash(accountDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var newAccount = new Account(accountDTO, Convert.ToBase64String(passwordSalt), Convert.ToBase64String(passwordHash));

                var responseNewAccount = await _accountService.CreateAccount(newAccount);
                if (responseNewAccount.Message is not null)
                    return new StandartResponse<ResponseAuthenticated> { Message = responseNewAccount.Message, StatusCode = responseNewAccount.StatusCode };

                newAccount = responseNewAccount.Data;
                var accsessToken = (await Authenticate(accountDTO)).Data;
                return new StandartResponse<ResponseAuthenticated>()
                {
                    Data = accsessToken,
                    StatusCode = StatusCode.AccountCreate
                };
            }
            return new StandartResponse<ResponseAuthenticated>()
            {
                Message = "Account with login already exists",
                StatusCode = responseAccount.StatusCode
            };
        }

        public async Task<BaseResponse<ResponseAuthenticated>> Authenticate(AccountDTO accountDTO)
        {
            var account = (await _accountService.GetAccount(x => x.Login == accountDTO.Login)).Data;
            if (account == null ||
                !_passwordService.VerifyPasswordHash(accountDTO.Password, Convert.FromBase64String(account.Password), Convert.FromBase64String(account.Salt)))
            {
                return new StandartResponse<ResponseAuthenticated>()
                {
                    Message = "account not found",
                    StatusCode = StatusCode.ErrorAuthenticate
                };
            }

            return new StandartResponse<ResponseAuthenticated>()
            {
                Data = _tokenService.GenerateAuthenticatedToken(account).Data,
                StatusCode = StatusCode.AccountAuthenticate
            };
        }

        public Task<BaseResponse<ResponseAuthenticated>> AuthenticateByRefreshToken(UserAccsessToken tokenAuth, string refreshToken)
        {
            var claimsRT = _tokenService.GetPrincipalFromExpiredToken(refreshToken, _jwtSettings.RefreshTokenSecretKey);
            var secretNumber = claimsRT?.Data?.Claims.FirstOrDefault(x => x.Type == CustomClaimType.SecretNumber);
            if (secretNumber != null)
            {
                // Проверка в Redis
            }
            return  Task.FromResult<BaseResponse<ResponseAuthenticated>>(new StandartResponse<ResponseAuthenticated> { Message = "Refresh token or access token is not valid" });
        }
    }
}
