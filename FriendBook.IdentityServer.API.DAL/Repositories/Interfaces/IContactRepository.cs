using FriendBook.IdentityServer.API.Domain.Entities;

namespace FriendBook.IdentityServer.API.DAL.Repositories.Interfaces
{
    public interface IContactRepository
    {
        public bool Clear(Account entity);
        public Task<bool> SaveAsync();
        public Account? Update(Account entity);
        public IQueryable<Account> GetAll();
    }
}
