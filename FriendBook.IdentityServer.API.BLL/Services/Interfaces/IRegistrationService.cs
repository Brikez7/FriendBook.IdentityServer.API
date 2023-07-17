using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.JWT;
using FriendBook.IdentityServer.API.Domain.Response;

namespace FriendBook.IdentityServer.API.BLL.Services.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<ResponseAuthenticated>> Registration(RequestAccount DTO);
        public Task<BaseResponse<ResponseAuthenticated>> Authenticate(RequestAccount DTO);
        public Task<BaseResponse<string>> AuthenticateByRefreshToken(DataAccessToken tokenAuth, string refreshToken);
    }
}
