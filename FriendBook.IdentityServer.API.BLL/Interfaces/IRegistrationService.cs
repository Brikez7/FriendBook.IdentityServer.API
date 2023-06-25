using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<AuthenticatedTokenResponse>> Registration(AccountDTO DTO);
        public Task<BaseResponse<AuthenticatedTokenResponse>> Authenticate(AccountDTO DTO);
        public Task<BaseResponse<AuthenticatedTokenResponse>> AuthenticateByRefreshToken(UserAccsessToken tokenAuth, string refreshToken);
    }
}
