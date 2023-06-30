using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Interfaces
{
    public interface IAccountService
    {
        public BaseResponse<IQueryable<Account>> GetAllAccounts();
        public Task<BaseResponse<Account>> GetAccount(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<bool>> AccountExists(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<Account>> CreateAccount(Account account);
        public Task<BaseResponse<Account>> UpdateAccount(AccountDTO account);
        public Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression);
        public Task<BaseResponse<Tuple<Guid, string>[]>> GetLogins(Guid[] usersIds);

    }
}
