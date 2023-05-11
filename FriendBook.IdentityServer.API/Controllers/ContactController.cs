using FriendBook.IdentityServer.API.BLL.Interfaces;
using FriendBook.IdentityServer.API.Domain.DTO;
using FriendBook.IdentityServer.API.Domain.Entities;
using FriendBook.IdentityServer.API.Domain.InnerResponse;
using HCL.IdentityServer.API.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FriendBook.IdentityServer.API.Controllers
{
    [Route("api/Contact[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IContactService _contactService;

        public ContactController(ILogger<ContactController> logger, IContactService contactService)
        {
            _logger = logger;
            _contactService = contactService;
        }
        [HttpGet("Get/")]
        public async Task<IResult> GetContactInformation(Guid Id)
        {
            var response = await _contactService.GetContact(x => x.Id == Id);
            if (response.Data == null)
            {
                response.Message = "entity not faund";
                response.StatusCode = Domain.InnerResponse.StatusCode.EntityNotFound;
                return Results.Json(response);
            }
            return Results.Json(response);
        }
        [HttpPut("Update/")]
        public async Task<IResult> UpdateContactInformation(UserContactDTO userContactDTO)
        {
            Guid userId;
            if (Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId).Value, out userId))
            {
                Account account = new Account(userContactDTO, userId);
                var response = await _contactService.UpdateContact(account);
                if (response.Data == null)
                {
                    response.Message = "entity not faund";
                    response.StatusCode = Domain.InnerResponse.StatusCode.EntityNotFound;
                    return Results.Json(response);
                }
                return Results.Json(response);
            }
            else
            {
                return Results.Json(new StandartResponse<Account>
                {
                    StatusCode = Domain.InnerResponse.StatusCode.AccountNotAuthenticate,
                    Message = "Coockie has empty"
                });
            }
        }
        [HttpDelete("Clear/")]
        public async Task<IResult> ClearContactInformation(Guid Id)
        {
            var response = await _contactService.ClearContact(x => x.Id == Id);
            if (!response.Data)
            {
                response.Message = "entity not faund";
                response.StatusCode = Domain.InnerResponse.StatusCode.EntityNotFound;
                return Results.Json(response);
            }
            return Results.Json(response);
        }
    }
}
