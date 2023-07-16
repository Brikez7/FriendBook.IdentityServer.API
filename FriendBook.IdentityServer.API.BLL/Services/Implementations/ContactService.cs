using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Services.Implementations
{
    public class ContactService : IContactService
    {
        private readonly IAccountRepository _accountRepository;
        protected readonly ILogger<IUserAccountService> _logger;
        public ContactService(IAccountRepository repository, ILogger<IUserAccountService> logger)
        {
            _accountRepository = repository;
            _logger = logger;
        }
        public async Task<BaseResponse<bool>> ClearContact(Expression<Func<Account, bool>> expression)
        {
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                return new StandartResponse<bool>()
                {
                    Message = "Contact not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            var accountIsDelete = _accountRepository.ClearContact(e => e.Id == entity.Id);
            await _accountRepository.SaveAsync();
            return new StandartResponse<bool>()
            {
                Data = accountIsDelete,
                StatusCode = StatusCode.ContactClear
            };
        }

        public async Task<BaseResponse<ResponseProfile[]>> GetProfiles(string login, Guid userId)
        {
            var entities = await _accountRepository.GetAll()
                                                   .Where(x => EF.Functions.Like(x.Login.ToLower(), $"%{login.ToLower()}%"))
                                                   .Select(x => new ResponseProfile((Guid)x.Id!, x.Login, x.FullName))
                                                   .ToListAsync();

            var account = entities.FirstOrDefault(x => x.Id == userId);
            if (account is not null)
                entities.Remove(account);

            if (entities.Count > 0 )
            {
                return new StandartResponse<ResponseProfile[]>()
                {
                    Data = entities.ToArray(),
                    StatusCode = StatusCode.ContactRead
                };
            }
            return new StandartResponse<ResponseProfile[]>()
            {
                Message = "Profiles not found",
                StatusCode = StatusCode.EntityNotFound
            };
        }


        public async Task<BaseResponse<UserContactDTO>> GetContact(Expression<Func<Account, bool>> expression)
        {
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                return new StandartResponse<UserContactDTO>()
                {
                    StatusCode = StatusCode.EntityNotFound,
                    Message = "Contact not found"
                };
            }
            return new StandartResponse<UserContactDTO>()
            {
                Data = new UserContactDTO(entity),
                StatusCode = StatusCode.ContactRead
            };
        }

        public async Task<BaseResponse<UserContactDTO>> UpdateContact(UserContactDTO contactDTO, string login, Guid idUser)
        {
            var existsAccount = await _accountRepository.GetAll()
                                                        .SingleOrDefaultAsync(x => x.Id == idUser);

            if (existsAccount is null)  
            {
                return new StandartResponse<UserContactDTO>()
                {
                    Message = "Account not found",
                    StatusCode = StatusCode.EntityNotFound,
                };
            }

            var account = new Account(contactDTO, idUser);

            existsAccount.Info = contactDTO.Info;
            existsAccount.FullName = contactDTO.FullName;
            existsAccount.Telephone = contactDTO.Telephone;
            existsAccount.Email = contactDTO.Email;
            existsAccount.Profession = contactDTO.Profession;

            await _accountRepository.SaveAsync();
            
            return new StandartResponse<UserContactDTO>()
            {
                Data = new UserContactDTO(existsAccount),
                StatusCode = StatusCode.ContactUpdate,
            };
        }
    }
    
}

