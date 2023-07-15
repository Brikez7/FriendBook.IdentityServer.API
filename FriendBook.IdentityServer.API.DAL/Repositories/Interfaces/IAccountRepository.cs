using FriendBook.IdentityServer.API.Domain.Entities;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.DAL.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        public Task<Account> AddAsync(Account entity);
        public bool ClearContact(Expression<Func<Account, bool>> expressionWhere);
        public Account Update(Account account);
        public bool Delete(Account entity);
        public IQueryable<Account> GetAll();
        public Task<bool> SaveAsync();
    }
}
 