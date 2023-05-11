using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
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
            try
            {
                var entity = await _contactRepository.GetAll().SingleOrDefaultAsync(expression);
                if (entity == null)
                {
                    return new StandartResponse<bool>()
                    {
                        Message = "entity not found"
                    };
                }
                var accountIsDelete = _contactRepository.Clear(entity);
                await _contactRepository.SaveAsync();
                return new StandartResponse<bool>()
                {
                    Data = accountIsDelete,
                    StatusCode = StatusCode.AccountDelete
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[DeleteAccount] : {ex.Message}");
                return new StandartResponse<bool>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<Account>> GetContact(Expression<Func<Account, bool>> expression)
        {
            try
            {
                var entity = await _contactRepository.GetAll().SingleOrDefaultAsync(expression);
                if (entity == null)
                {
                    return new StandartResponse<Account>()
                    {
                        Message = "entity not found"
                    };
                }
                return new StandartResponse<Account>()
                {
                    Data = entity,
                    StatusCode = StatusCode.AccountRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAccount] : {ex.Message}");
                return new StandartResponse<Account>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<Account>> UpdateContact(Account account)
        {
            try
            {
                var updatedAccount = _contactRepository.Update(account);
                await _contactRepository.SaveAsync();
                return new StandartResponse<Account>()
                {
                    Data = updatedAccount,
                    StatusCode = StatusCode.AccountUpdate,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UpdateAccount] : {ex.Message}");
                return new StandartResponse<Account>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }
    }
}
