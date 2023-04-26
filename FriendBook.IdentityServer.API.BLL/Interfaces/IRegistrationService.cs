using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<(string, Guid)>> Registration(AccountDTO DTO);
        public Task<BaseResponse<(string, Guid)>> Authenticate(AccountDTO DTO);
        public string GetToken(Account account);
    }
}
