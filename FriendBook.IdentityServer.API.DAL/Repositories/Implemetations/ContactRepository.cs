using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.Entities;
using System.Xml;

namespace FriendBook.IdentityServer.API.DAL.Repositories.Implemetations
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDBContext _db;

        public ContactRepository(AppDBContext db)
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

        public Account Update(Account entity)
        {
            _db.Accounts.Attach(entity);
            _db.Entry(entity).Property(x => new {x.Info,x.FullName,x.Telephone,x.Email,x.Company,x.Profession }).IsModified = true;
            return entity;
        }
    }
}
