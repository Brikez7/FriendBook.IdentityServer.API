using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
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

        public async Task<BaseResponse<ProfileDTO[]>> GetAllProphile(string login, Guid id)
        {
            try
            {
                var entities = _contactRepository.GetAll()
                                                 .Where(x => EF.Functions.Like(x.Login.ToLower(), $"%{login.ToLower()}%"))
                                                 .Select(x => new ProfileDTO(x.Id,x.Login,x.FullName))
                                                 .ToList();

                entities.RemoveAll(x => x.Id == id);
                var array = entities.ToArray();
                if (array == null || array.Count() == 0)
                {
                    return new StandartResponse<ProfileDTO[]>()
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Message = "entity not found"
                    };
                }
                return new StandartResponse<ProfileDTO[]>()
                {
                    Data = array,
                    StatusCode = StatusCode.AccountRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAccount] : {ex.Message}");
                return new StandartResponse<ProfileDTO[]>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }


        public async Task<BaseResponse<UserContactDTO>> GetContact(Expression<Func<Account, bool>> expression)
        {
            try
            {
                var entity = await _contactRepository.GetAll().SingleOrDefaultAsync(expression);
                if (entity == null)
                {
                    return new StandartResponse<UserContactDTO>()
                    {
                        StatusCode = StatusCode.InternalServerError,
                        Message = "entity not found"
                    };
                }
                return new StandartResponse<UserContactDTO>()
                {
                    Data = new UserContactDTO(entity),
                    StatusCode = StatusCode.AccountRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAccount] : {ex.Message}");
                return new StandartResponse<UserContactDTO>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<UserContactDTO>> UpdateContact(Account account)
        {
            try
            {
                var updatedAccount = _contactRepository.Update(account);

                if (updatedAccount is null) 
                {
                    return new StandartResponse<UserContactDTO>()
                    {
                        Message = "Account not found",
                        StatusCode = StatusCode.InternalServerError,
                    };
                }

                await _contactRepository.SaveAsync();

                return new StandartResponse<UserContactDTO>()
                {
                    Data = new UserContactDTO(updatedAccount),
                    StatusCode = StatusCode.AccountUpdate,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[UpdateAccount] : {ex.Message}");
                return new StandartResponse<UserContactDTO>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }
    }
}
