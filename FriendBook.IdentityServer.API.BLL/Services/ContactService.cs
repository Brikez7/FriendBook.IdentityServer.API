﻿using FriendBook.IdentityServer.API.BLL.Interfaces;
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
                    Message = "entity not found",
                    StatusCode = StatusCode.InternalServerError
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

        public async Task<BaseResponse<ResponseProfile[]>> GetAllProphile(string login, Guid id)
        {
            var entities = await _contactRepository.GetAll()
                                                .Where(x => EF.Functions.Like(x.Login.ToLower(), $"%{login.ToLower()}%") && x.Id != id)
                                                .Select(x => new ResponseProfile((Guid)x.Id!, x.Login, x.FullName))
                                                .ToListAsync();

            var array = entities.ToArray();
            if (array == null || array.Length == 0)
            {
                return new StandartResponse<ResponseProfile[]>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Message = "entity not found"
                };
            }
            return new StandartResponse<ResponseProfile[]>()
            {
                Data = array,
                StatusCode = StatusCode.AccountRead
            };
        }


        public async Task<BaseResponse<UserContactDTO>> GetContact(Expression<Func<Account, bool>> expression)
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

        public async Task<BaseResponse<UserContactDTO>> UpdateContact(UserContactDTO contactDTO, string login, Guid idUser)
        {
            if (contactDTO.Login == "") 
            {
                return new StandartResponse<UserContactDTO>()
                {
                    Message = "Login is null",
                    StatusCode = StatusCode.InternalServerError,
                };
            }

            var userId = await _contactRepository.GetAll()
                                                 .Select(x => new { x.Id,x.Login})
                                                 .SingleOrDefaultAsync(x => x.Login == contactDTO.Login && idUser == x.Id);

            if (userId is null && login != contactDTO.Login)  
            {
                return new StandartResponse<UserContactDTO>()
                {
                    Message = "an account with this username already exists",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
            var account = new Account(contactDTO, (Guid)userId.Id);
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
    }
    
}

