using FriendBook.IdentityServer.API.Domain.DTO.AcouuntsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IContactService
    {
        public Task<BaseResponse<UserContactDTO>> GetContact(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<bool>> ClearContact(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<UserContactDTO>> UpdateContact(UserContactDTO account, string login);
        public Task<BaseResponse<ProfileDTO[]>> GetAllProphile(string login, Guid id);

    }
}
