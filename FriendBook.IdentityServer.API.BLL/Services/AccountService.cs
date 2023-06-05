using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.DAL.Repositories.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO.AcouuntsDTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace FriendBook.IdentityServer.API.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        protected readonly ILogger<IAccountService> _logger;
        public AccountService(IAccountRepository repository, ILogger<IAccountService> logger)
        {
            _accountRepository = repository;
            _logger = logger;
        }
        public async Task<BaseResponse<Account>> CreateAccount(Account account)
        {
            var createdAccount = await _accountRepository.AddAsync(account);
            await _accountRepository.SaveAsync();
            return new StandartResponse<Account>()
            {
                Data = createdAccount,
                StatusCode = StatusCode.AccountCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteAccount(Expression<Func<Account, bool>> expression)
        {
            var entity = await _accountRepository.GetAll().SingleOrDefaultAsync(expression);
            if (entity == null)
            {
                return new StandartResponse<bool>()
                {
                    Message = "entity not found",
                    StatusCode = StatusCode.InternalServerError,
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
                    Message = "entity not found",
                    StatusCode = StatusCode.InternalServerError
                };
            }
            return new StandartResponse<Account>()
            {
                Data = entity,
                StatusCode = StatusCode.AccountRead
            };
        }


        public BaseResponse<IQueryable<Account>> GetAllAccounts()
        {
            var contents = _accountRepository.GetAll();
            if (contents == null)
            {
                return new StandartResponse<IQueryable<Account>>()
                {
                    Message = "entity not found",
                    StatusCode= StatusCode.InternalServerError,
                };
            }
            return new StandartResponse<IQueryable<Account>>()
            {
                Data = contents,
                StatusCode = StatusCode.AccountRead
            };
        }

        public async Task<BaseResponse<Account>> UpdateAccount(AccountDTO accountDTO)
        {
            var account = new Account(accountDTO);

            var updatedAccount = _accountRepository.Update(account);
            await _accountRepository.SaveAsync();

            return new StandartResponse<Account>()
            {
                Data = updatedAccount,
                StatusCode = StatusCode.AccountUpdate,
            };
            
        }
        public async Task<BaseResponse<Tuple<Guid, string>[]>> GenLogins(Guid[] usersIds)
        {
            var loginWithId = await _accountRepository.GetAll()
                                                      .Select(x => new { x.Id, x.Login })
                                                      .ToListAsync();

            var SearchedLogins = loginWithId.Where(x => usersIds.Any(id => id == x.Id))
                                           .Select(x => new Tuple<Guid, string>((Guid)x.Id, x.Login))
                                           .ToArray();

            return new StandartResponse<Tuple<Guid, string>[]>
            {
                Data = SearchedLogins
            };
        }
    }
}
