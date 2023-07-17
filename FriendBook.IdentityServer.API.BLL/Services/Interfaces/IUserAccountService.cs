using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.Response;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Services.Interfaces
{
    public interface IUserAccountService
    {
        public Task<BaseResponse<Account>> GetAccount(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<bool>> AccountExists(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<Tuple<Guid, string>[]>> GetLogins(Guid[] usersIds);

    }
}
