using FriendBook.IdentityServer.API.Domain.DTO.AcouuntsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using System.Security.Claims;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IRegistrationService
    {
        public Task<BaseResponse<string>> Registration(AccountDTO DTO);
        public Task<BaseResponse<string>> Authenticate(AccountDTO DTO);
    }
}
