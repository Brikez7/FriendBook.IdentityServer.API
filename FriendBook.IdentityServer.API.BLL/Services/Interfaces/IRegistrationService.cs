using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;

namespace FriendBook.IdentityServer.API.BLL.Services.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<ResponseAuthenticate>> Registration(RequestNewAccount DTO);
        public Task<BaseResponse<ResponseAuthenticate>> Authenticate(RequestNewAccount DTO);
        public Task<BaseResponse<string>> AuthenticateByRefreshToken(DataAccessToken tokenAuth, string refreshToken);
    }
}
