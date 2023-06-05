using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.Entities;

namespace FriendBook.IdentityServer.API.DAL.Repositories.Implemetations
{
    public class ContactRepository : IContactRepository
    {
        private readonly IdentityContext _db;

        public ContactRepository(IdentityContext db)
        {
            _db = db;
        }

        public bool Clear(Account entity)
        {
            var contact = _db.Accounts.Where(x => x.Id == entity.Id).Single();

            contact.Info = null;
            contact.FullName = null;
            contact.Telephone = null;
            contact.Email = null;
            contact.Company = null;
            contact.Profession = null;

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

        public Account? Update(Account entity)
        {
            var existingEntity = _db.Accounts.SingleOrDefault(x => x.Id == entity.Id);
            if (existingEntity != null)
            {
                existingEntity.Info = entity.Info;
                existingEntity.FullName = entity.FullName;
                existingEntity.Telephone = entity.Telephone;
                existingEntity.Email = entity.Email;
                existingEntity.Login = entity.Login;
                existingEntity.Profession = entity.Profession;

                return entity;
            }
            else 
            {
                return null;
            }
        }
    }
}
