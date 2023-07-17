using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using FriendBook.IdentityServer.API.Domain.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Services.Implementations
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IAccountRepository _accountRepository;
        protected readonly ILogger<IUserAccountService> _logger;
        public UserAccountService(IAccountRepository repository, ILogger<IUserAccountService> logger)
        {
            _accountRepository = repository;
            _logger = logger;
        }

        public async Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression)
        {
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                return new StandartResponse<bool>()
                {
                    Message = "entity not found",
                    StatusCode = StatusCode.EntityNotFound,
                };
            }
            var accountIsDelete = _accountRepository.Delete(entity);
            await _accountRepository.SaveAsync();
            return new StandartResponse<bool>()
            {
                Data = accountIsDelete,
                StatusCode = StatusCode.AccountDelete
            };
        }

        public async Task<BaseResponse<Account>> GetAccount(Expression<Func<Account, bool>> expression)
        {
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                return new StandartResponse<Account>()
                {
                    Message = "Account not found",
                    StatusCode = StatusCode.EntityNotFound
                };
            }
            return new StandartResponse<Account>()
            {
                Data = entity,
                StatusCode = StatusCode.AccountRead
            };
        }
        public async Task<BaseResponse<Tuple<Guid, string>[]>> GetLogins(Guid[] usersIds)
        {
            var loginWithId = await _accountRepository.GetAll()
                                                      .Select(x => new { x.Id, x.Login })
                                                      .ToListAsync();

            var SearchedLogins = loginWithId.Where(x => usersIds.Any(id => id == x.Id))
                                            .Select(x => new Tuple<Guid, string>((Guid)x.Id!, x.Login))
                                            .ToArray();

            return new StandartResponse<Tuple<Guid, string>[]>
            {
                Data = SearchedLogins
            };
        }

        public async Task<BaseResponse<bool>> AccountExists(Expression<Func<Account, bool>> expression)
        {
            var accountExists = await _accountRepository.GetAll().AnyAsync(expression);
            return new StandartResponse<bool>()
            {
                Data = accountExists,
                StatusCode = StatusCode.AccountRead
            };
        }
    }
}
