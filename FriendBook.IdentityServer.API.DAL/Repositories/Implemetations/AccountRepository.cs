using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.Entities;

namespace FriendBook.IdentityServer.API.DAL.Repositories.Implemetations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IdentityServerContext _db;

        public AccountRepository(IdentityServerContext db)
        {
            _db = db;
        }

        public async Task<Account> AddAsync(Account entity)
        {
            var createdEntity = await _db.Accounts.AddAsync(entity);
            return createdEntity.Entity;
        }

        public bool Delete(Account entity)
        {
            _db.Accounts.Remove(entity);
            return true;
        }

        public IQueryable<Account> GetAll()
        {
            return _db.Accounts;
        }

        public async Task<bool> SaveAsync()
        {
            await _db.SaveChangesAsync();
            return true;
        }

        public Account Update(Account entity)
        {
            var updatedEntity = _db.Accounts.Update(entity);
            return updatedEntity.Entity;
        }
    }
}
