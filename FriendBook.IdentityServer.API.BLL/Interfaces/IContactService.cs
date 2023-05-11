using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IContactService
    {
        public Task<BaseResponse<Account>> GetContact(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<bool>> ClearContact(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<Account>> UpdateContact(Account account);

    }
}
