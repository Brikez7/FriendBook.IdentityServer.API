using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.UserToken;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<ResponseAuthenticated>> Registration(RequestAccount DTO);
        public Task<BaseResponse<ResponseAuthenticated>> Authenticate(RequestAccount DTO);
        public Task<BaseResponse<string>> GetAccessToken(TokenAuth tokenAuth, string refreshToken);
    }
}
