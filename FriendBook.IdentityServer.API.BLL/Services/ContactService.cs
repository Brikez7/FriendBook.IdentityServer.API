using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain;
using FriendBook.IdentityServer.API.Domain.DTO.AccountsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        protected readonly ILogger<IAccountService> _logger;
        public ContactService(IContactRepository repository, ILogger<IAccountService> logger)
        {
            _contactRepository = repository;
            _logger = logger;
        }
        public async Task<BaseResponse<bool>> ClearContact(Expression<Func<Account, bool>> expression)
        {
            var entity = await _contactRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                return new StandartResponse<bool>()
                {
                    Message = "Contact not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            var accountIsDelete = _contactRepository.Clear(entity);
            await _contactRepository.SaveAsync();
            return new StandartResponse<bool>()
            {
                Data = accountIsDelete,
                StatusCode = StatusCode.ContactClear
            };
        }

        public async Task<BaseResponse<ResponseProfile[]>> GetProfiles(string login, Guid userId)
        {
            var entities = await _contactRepository.GetAll()
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
            var entity = await _contactRepository.GetAll().SingleOrDefaultAsync(expression);
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
            var userId = await _contactRepository.GetAll()
                                                 .Select(x => new { x.Id,x.Login})
                                                 .SingleOrDefaultAsync(x => x.Login == contactDTO.Login);

            if (userId is not null && userId?.Id != idUser)  
            {
                return new StandartResponse<UserContactDTO>()
                {
                    Message = "an account with username already exists",
                    StatusCode = StatusCode.UserAlreadyExists,
                };
            }

            var account = new Account(contactDTO, idUser);
            var updatedAccount = _contactRepository.Update(account);

            if (updatedAccount is null) 
            {
                return new StandartResponse<UserContactDTO>()
                {
                    Message = "Account not found",
                    StatusCode = StatusCode.EntityNotFound,
                };
            }

            await _contactRepository.SaveAsync();

            return new StandartResponse<UserContactDTO>()
            {
                Data = new UserContactDTO(updatedAccount),
                StatusCode = StatusCode.ContactUpdate,
            };
        }
    }
    
}

